<#
.SYNOPSIS
    Importa una base de datos legacy (SQL Dump).
#>

param (
    [string]$SourceSqlDump,
    [string]$StagingDbName,
    [string]$MigrationScript,
    [string]$TargetDbName,
    
    # Remote / Cloud Options
    [string]$DbHost,
    [string]$DbPort,
    [string]$ContainerName,
    [string]$DbUser,
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSAvoidUsingPlainTextForPassword", "")]
    [SecureString]$DbPassword,
    [bool]$UseDockerExec = $true,
    [switch]$SkipSchemaInit = $false
)

$ErrorActionPreference = "Stop"

# 0. Load Environment Variables from .env file
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$EnvFile = Resolve-Path "$ScriptDir\..\..\.env" -ErrorAction SilentlyContinue
$EnvVars = @{}

if ($EnvFile -and (Test-Path $EnvFile)) {
    Write-Host "Loading configuration from .env file..." -ForegroundColor DarkGray
    Get-Content $EnvFile | ForEach-Object {
        if ($_ -match "^(.*?)=(.*)$" -and -not $_.StartsWith("#")) {
            $key = $matches[1].Trim()
            $val = $matches[2].Trim()
            $EnvVars[$key] = $val
        }
    }
}

# 0.1 Apply Defaults from .env (if parameters not explicitly passed)
if (-not $PsBoundParameters.ContainsKey('DbUser') -and $EnvVars.ContainsKey('DB_USER')) { $DbUser = $EnvVars['DB_USER'] }
# Note: For DbHost, if UseDockerExec is true, we generally want 127.0.0.1 for local tools, ignoring DB_HOST=db from .env
if (-not $PsBoundParameters.ContainsKey('DbHost') -and -not $UseDockerExec -and $EnvVars.ContainsKey('DB_HOST')) { $DbHost = $EnvVars['DB_HOST'] }
if (-not $PsBoundParameters.ContainsKey('DbPort') -and $EnvVars.ContainsKey('DB_PORT')) { $DbPort = $EnvVars['DB_PORT'] }
if (-not $PsBoundParameters.ContainsKey('ContainerName') -and $EnvVars.ContainsKey('CONTAINER_NAME')) { $ContainerName = $EnvVars['CONTAINER_NAME'] }

# Fallback defaults if not set in param or env
if ([string]::IsNullOrWhiteSpace($DbUser)) { $DbUser = "root" }
if ([string]::IsNullOrWhiteSpace($DbHost)) { $DbHost = "127.0.0.1" }
if ([string]::IsNullOrWhiteSpace($DbPort)) { $DbPort = "3306" }
if ([string]::IsNullOrWhiteSpace($ContainerName)) { $ContainerName = "consulcon_db" }

# 0.2 Apply Migration Defaults from .env
if ([string]::IsNullOrWhiteSpace($SourceSqlDump) -and $EnvVars.ContainsKey('MIGRATION_SOURCE_DUMP')) { $SourceSqlDump = $EnvVars['MIGRATION_SOURCE_DUMP'] }
if ([string]::IsNullOrWhiteSpace($StagingDbName) -and $EnvVars.ContainsKey('MIGRATION_STAGING_DB')) { $StagingDbName = $EnvVars['MIGRATION_STAGING_DB'] }
if ([string]::IsNullOrWhiteSpace($MigrationScript) -and $EnvVars.ContainsKey('MIGRATION_SCRIPT')) { $MigrationScript = $EnvVars['MIGRATION_SCRIPT'] }
if ([string]::IsNullOrWhiteSpace($TargetDbName) -and $EnvVars.ContainsKey('MIGRATION_TARGET_DB')) { $TargetDbName = $EnvVars['MIGRATION_TARGET_DB'] }
# Fallback default for TargetDbName if still empty
if ([string]::IsNullOrWhiteSpace($TargetDbName)) { $TargetDbName = "consulcon" }

# 0.3 Validate Required Parameters
$MissingParams = @()
if ([string]::IsNullOrWhiteSpace($SourceSqlDump)) { $MissingParams += "SourceSqlDump (or MIGRATION_SOURCE_DUMP in .env)" }
if ([string]::IsNullOrWhiteSpace($StagingDbName)) { $MissingParams += "StagingDbName (or MIGRATION_STAGING_DB in .env)" }
if ([string]::IsNullOrWhiteSpace($MigrationScript)) { $MissingParams += "MigrationScript (or MIGRATION_SCRIPT in .env)" }

if ($MissingParams.Count -gt 0) {
    Write-Error "Missing required parameters: $($MissingParams -join ', ')"
}

# Handle optional SecureString parameter default
if (-not $DbPassword) {
    # Try to get password from .env, fallback to "root"
    $envPass = if ($EnvVars.ContainsKey('DB_PASSWORD')) { $EnvVars['DB_PASSWORD'] } else { "root" }
    
    # Check if root password is provided if user is root (fallback handling)
    if ($DbUser -eq "root" -and $EnvVars.ContainsKey('DB_ROOT_PASSWORD')) {
         # Prefer ROOT password for root user if explicitly set? Usually DB_PASSWORD is the user's password.
         # We stick to DB_PASSWORD for consistency with docker-compose service "api".
    }

    $DbPassword = ConvertTo-SecureString $envPass -AsPlainText -Force
}

# Helper to unwrap SecureString for external processes (docker/mysql) that require plain text args
$PlainDbPassword = (New-Object System.Net.NetworkCredential([string]::Empty, $DbPassword)).Password

Write-Host ">>> Starting Legacy Migration..." -ForegroundColor Cyan
Write-Host "Source Dump: $SourceSqlDump"
Write-Host "Staging DB: $StagingDbName"
Write-Host "Migration Script: $MigrationScript"
Write-Host "Target DB: $TargetDbName"

# 1. Validate Files
if (-not (Test-Path $SourceSqlDump)) {
    Write-Error "File not found: SourceSqlDump: $SourceSqlDump"
}
if (-not (Test-Path $MigrationScript)) {
    Write-Error "File not found: MigrationScript: $MigrationScript"
}

# Helper for executing MySQL commands
function Invoke-MySqlCmd {
    param (
        [string]$Query,
        [string]$File,
        [string]$Database
    )
    
    if ($UseDockerExec) {
        # Local Docker Execution
        $baseArgs = @("exec", "-i", $ContainerName, "mysql", "-u$DbUser", "-p$PlainDbPassword")
        if ($Database) { $baseArgs += $Database }
        
        if ($File) {
            # Pipe file content
            Get-Content $File | & docker $baseArgs
        } elseif ($Query) {
            # Pipe string query
            $Query | & docker $baseArgs
        }
    } else {
        # Remote / Native Execution (Requires mysql client installed locally)
        # Note: Powershell might need explicit path to mysql.exe if not in PATH
        $baseArgs = @("-h", $DbHost, "-P", $DbPort, "-u", $DbUser, "-p$PlainDbPassword")
        if ($Database) { $baseArgs += $Database }
        
        if ($File) {
             # Native mysql < file
             Get-Content $File | & mysql $baseArgs
        } elseif ($Query) {
             $baseArgs += @("-e", $Query)
             & mysql $baseArgs
        }
    }
    
    if ($LASTEXITCODE -ne 0) {
        throw "MySQL Command Failed with Exit Code: $LASTEXITCODE"
    }
}

# 2. Create Staging DB
Write-Host "`n1. Preparing Staging DB ($StagingDbName)..." -ForegroundColor Yellow
$CreateDbCmd = "DROP DATABASE IF EXISTS $StagingDbName; CREATE DATABASE $StagingDbName CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"
try {
    Invoke-MySqlCmd -Query $CreateDbCmd
    Write-Host "Database $StagingDbName created/reset." -ForegroundColor Green
} catch {
    Write-Error "Error creating DB: $_"
}

# 3. Import Legacy Dump
Write-Host "`n2. Importing Legacy Dump to $StagingDbName (This may take a while)..." -ForegroundColor Yellow
try {
    Invoke-MySqlCmd -Database $StagingDbName -File $SourceSqlDump
    Write-Host "Import successful." -ForegroundColor Green
} catch {
    Write-Error "Error importing SQL Dump: $_"
}

# 4. Preparing Transformation Script
Write-Host "`n3. Preparing Transformation Script..." -ForegroundColor Yellow
$MigrationTempFile = "$env:TEMP\migrated_temp.sql"
try {
    $MigrationContent = Get-Content $MigrationScript -Raw
    
    if ($MigrationContent -match "\{\{STAGING_DB\}\}") {
        $MigrationContent = $MigrationContent -replace "\{\{STAGING_DB\}\}", $StagingDbName
        Write-Host "Placeholder {{STAGING_DB}} replaced by $StagingDbName." -ForegroundColor Green
    }

    if ($MigrationContent -match "\{\{TARGET_DB\}\}") {
        $MigrationContent = $MigrationContent -replace "\{\{TARGET_DB\}\}", $TargetDbName
        Write-Host "Placeholder {{TARGET_DB}} replaced by $TargetDbName." -ForegroundColor Green
    }
    
    Set-Content -Path $MigrationTempFile -Value $MigrationContent
} catch {
    Write-Error "Failed to process migration script placeholders: $_"
}

# 5. Ensure Target DB Exists & Has Schema (Using API Migration)
Write-Host "`n4. Initializing Target DB Scheme ($TargetDbName)..." -ForegroundColor Yellow

# Run API container briefly to trigger EnsureCreated/Migrate
# If using Remote DB ($UseDockerExec = $false), we MUST tell the container where the DB is.
# If using Local DB, DB_HOST=db (from docker-compose network).

$ApiDbHost = if ($UseDockerExec) { "db" } else { $DbHost } 

if ($SkipSchemaInit) {
    Write-Host "Skipping Schema Initialization (assumed target DB exists or managed by Cloud API)..." -ForegroundColor Cyan
} else {
    Write-Host "Running API util to create schema in $TargetDbName (Host: $ApiDbHost)..."
    
    # Override User/Password/Host for the migration runner
    $EnvParams = @("-e", "DB_NAME=$TargetDbName", "-e", "ASPNETCORE_ENVIRONMENT=Development", "-e", "DB_HOST=$ApiDbHost", "-e", "DB_PORT=$DbPort", "-e", "DB_USER=$DbUser", "-e", "DB_PASSWORD=$PlainDbPassword")
    
    # Using 'docker-compose run' 
    $ComposeArgs = @("run", "--rm") + $EnvParams + @("api", "dotnet", "Consulcon.API.dll", "--migrate-only")
    
    $CreateSchemaProcess = Start-Process -FilePath "docker-compose" -ArgumentList $ComposeArgs -Wait -NoNewWindow -PassThru
    
    if ($CreateSchemaProcess.ExitCode -eq 0) {
        Write-Host "Schema initialized successfully." -ForegroundColor Green
    } else {
        Write-Warning "Schema initialization might have failed (Exit Code: $($CreateSchemaProcess.ExitCode)). Checks logs above."
    }
}


# 6. Execute Transformation
Write-Host "`n5. Executing Transformation to $TargetDbName..." -ForegroundColor Yellow
try {
    Invoke-MySqlCmd -Database $TargetDbName -File $MigrationTempFile
    
    # 7. Post-Migration: Hash Passwords
    Write-Host "`n6. Hashing Plain Text Passwords..." -ForegroundColor Yellow
try {
    # Determine Project Root to locate PasswordHasher
    $ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
    # $ScriptDir is ...\scripts\utils
    # We need ...\scripts\PasswordHasher
    $HasherProject = Resolve-Path "$ScriptDir\..\PasswordHasher\PasswordHasher.csproj"

    if (Test-Path $HasherProject) {
        Write-Host "Running PasswordHasher utility..."
        
        if ($UseDockerExec) {
             # Running from host against local container -> use mapped port 3310
             $HasherPort = "3310" 
        } else {
             # Remote -> use designated port
             $HasherPort = $DbPort
        }

        dotnet run --project "$HasherProject" -- $DbHost $HasherPort $DbUser $PlainDbPassword
        
        Write-Host "Password hashing completed." -ForegroundColor Green
    } else {
        Write-Warning "PasswordHasher project not found at $HasherProject. Skipping hashing."
    }
} catch {
    Write-Error "Failed to hash passwords: $_"
}

# 7. Post-Migration: Sync Users to Master
Write-Host "`n7. Syncing Users to Master DB..." -ForegroundColor Yellow
try {
    $SyncerProject = Resolve-Path "$ScriptDir\..\UserSyncer\UserSyncer.csproj"

    if (Test-Path $SyncerProject) {
        Write-Host "Running UserSyncer utility..."
        
        if ($UseDockerExec) {
             $SyncerPort = "3310" 
        } else {
             $SyncerPort = $DbPort
        }

        dotnet run --project "$SyncerProject" -- $DbHost $SyncerPort $DbUser $PlainDbPassword
        
        Write-Host "User sync completed." -ForegroundColor Green
    } else {
        Write-Warning "UserSyncer project not found at $SyncerProject. Skipping sync."
    }
} catch {
    Write-Error "Failed to sync users: $_"
}

Write-Host "`nSUCCESS: MIGRATION COMPLETED" -ForegroundColor Magenta
 } catch {
    Write-Error "Error executing transformation: $_"
} finally {
    if (Test-Path $MigrationTempFile) { Remove-Item $MigrationTempFile }
}
