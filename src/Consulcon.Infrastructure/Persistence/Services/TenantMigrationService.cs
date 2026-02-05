using Consulcon.Application.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Consulcon.Infrastructure.Persistence.Services
{
    public class TenantMigrationService(
        IConfiguration configuration, 
        ILogger<TenantMigrationService> logger,
        IServiceProvider serviceProvider) : ITenantMigrationService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<TenantMigrationService> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task MigrateTenantDatabaseAsync(string tenantDbName)
        {
            // 1. Construct Connection String for Specific Tenant DB
            var connectionString = GetConnectionStringForDatabase(tenantDbName);

            // 2. Locate Migration Scripts
            var migrationFolder = Path.Combine(AppContext.BaseDirectory, "migrations");
            
            // Fallback for local dev if not found in bin
            if (!Directory.Exists(migrationFolder))
            {
                 var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;
                 if (projectRoot != null)
                 {
                     migrationFolder = Path.Combine(projectRoot, "scripts", "database", "migrations", "active");
                 }
            }

            if (!Directory.Exists(migrationFolder))
            {
                _logger.LogWarning("Migration folder not found at {Path}. Skipping tenant migrations for {DbName}.", migrationFolder, tenantDbName);
                return;
            }

            var sqlFiles = Directory.GetFiles(migrationFolder, "*.sql").OrderBy(f => f).ToList();

            if (sqlFiles.Count == 0)
            {
                _logger.LogInformation("No pending migration scripts found for {DbName}.", tenantDbName);
                // Even if no scripts, valid tenant DB might need sync? Usually migration implies creation/update.
                // We'll proceed to sync anyway if DB exists.
            }

            try 
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                // Ensure History Table
                await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS `__ExternalMigrationsHistory` (
                        `MigrationId` varchar(150) NOT NULL,
                        `AppliedOn` datetime NOT NULL,
                        PRIMARY KEY (`MigrationId`)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;");

                foreach (var file in sqlFiles)
                {
                    var fileName = Path.GetFileName(file);
                    
                    var isExecuted = await connection.ExecuteScalarAsync<bool>(
                        "SELECT COUNT(1) FROM __ExternalMigrationsHistory WHERE MigrationId = @MigrationId", 
                        new { MigrationId = fileName });

                    if (!isExecuted)
                    {
                        var scriptContent = await File.ReadAllTextAsync(file);
                        
                        using var transaction = await connection.BeginTransactionAsync();
                        try
                        {
                            await connection.ExecuteAsync(scriptContent, transaction: transaction);
                            
                            await connection.ExecuteAsync(
                                "INSERT INTO __ExternalMigrationsHistory (MigrationId, AppliedOn) VALUES (@MigrationId, NOW())",
                                new { MigrationId = fileName },
                                transaction: transaction);

                            await transaction.CommitAsync();
                            _logger.LogInformation("✅ Migration {FileName} applied to {DbName}.", fileName, tenantDbName);
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError(ex, "❌ Error executing migration {FileName} on {DbName}", fileName, tenantDbName);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to apply migrations to {DbName}", tenantDbName);
                 throw;
            }

            // 3. SYNC WITH MASTER DB
            try
            {
                await SyncToMasterAsync(tenantDbName, connectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync tenant {DbName} with Master Database.", tenantDbName);
                // We typically don't want to fail the whole migration if sync fails, but it's critical for login.
                // For now, log error.
            }
        }

        private async Task SyncToMasterAsync(string tenantDbName, string tenantConnectionString)
        {
            using var scope = _serviceProvider.CreateScope();
            var masterContext = scope.ServiceProvider.GetRequiredService<ConsulconDbContext>();
            
            // A. Register Condominium
            // Extract simplified TenantId (e.g. "db_condominio_foret" -> "foret")
            var tenantId = tenantDbName.Replace("db_condominio_", "").Replace("db_", ""); // Simple cleanup
            
            var condoMaster = await masterContext.CondominiosMaster.FirstOrDefaultAsync(c => c.TenantId == tenantId);
            
            if (condoMaster == null)
            {
                condoMaster = new Consulcon.Domain.Entities.Master.CondominioMaster
                {
                    TenantId = tenantId,
                    Nombre = tenantId.ToUpper(), // Temporary name
                    ConnectionString = tenantDbName 
                };
                masterContext.CondominiosMaster.Add(condoMaster);
                await masterContext.SaveChangesAsync();
                _logger.LogInformation("Registered new condominium in Master: {TenantId}", tenantId);
            }

            // B. Sync Users
            // Query Tenant Users using Dapper
            using var tenantConnection = new MySqlConnection(tenantConnectionString);
            
            // Validate if Usuario table exists (might be empty if new but schemas usually create it)
            // If migration failed previously, we might not have it.
            try 
            {
                 var tenantUsers = await tenantConnection.QueryAsync<dynamic>("SELECT IdUsuario, Username, PasswordHash, IdPersona FROM Usuario");
            
                foreach (var tUser in tenantUsers)
                {
                    string username = tUser.Username;
                    string passwordHash = tUser.PasswordHash;

                    // Check if user exists in Master
                    var masterUser = await masterContext.UsuariosMaster
                        .Include(u => u.Condominios)
                        .FirstOrDefaultAsync(u => u.Username == username);

                    if (masterUser == null)
                    {
                        masterUser = new Consulcon.Domain.Entities.Master.UsuarioMaster
                        {
                            Username = username,
                            PasswordHash = passwordHash,
                            Email = $"{username}@consulcon.com", 
                            EsSuperAdmin = false
                        };
                        masterContext.UsuariosMaster.Add(masterUser);
                        await masterContext.SaveChangesAsync();
                        _logger.LogInformation("Created new Global User: {Username}", username);
                    }

                    // Link User to Condominium
                    if (!masterUser.Condominios.Any(uc => uc.CondominioId == condoMaster.Id))
                    {
                        masterContext.UsuarioCondominios.Add(new Consulcon.Domain.Entities.Master.UsuarioCondominio
                        {
                            UsuarioId = masterUser.Id,
                            CondominioId = condoMaster.Id,
                            RolInicial = "Usuario" 
                        });
                        await masterContext.SaveChangesAsync();
                         _logger.LogInformation("Linked User {Username} to Tenant {TenantId}", username, tenantId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Could not sync users from tenant {TenantId}. Maybe table 'Usuario' does not exist yet? Error: {Message}", tenantId, ex.Message);
            }
        }

        private string GetConnectionStringForDatabase(string databaseName)
        {
            var dbHost = _configuration["DB_HOST"];
            var dbPort = _configuration["DB_PORT"] ?? "3306";
            var dbUser = _configuration["DB_USER"];
            var dbPassword = _configuration["DB_PASSWORD"];

            return $"Server={dbHost};Port={dbPort};Database={databaseName};User={dbUser};Password={dbPassword};";
        }
    }
}
