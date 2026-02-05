using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.Data;

using Consulcon.Infrastructure.Persistence;

namespace Consulcon.Infrastructure.Persistence.Services
{
    public class DatabaseMigrationInitializer(
        ILogger<DatabaseMigrationInitializer> logger,
        IServiceProvider serviceProvider)
    {
        private readonly ILogger<DatabaseMigrationInitializer> _logger = logger;
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task InitializeAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            // 1. Initialize MASTER Database
            try
            {
                // We reuse ConsulconDbContext but relies on it being configured for Master DB by default or logic
                // However, since we are in the same scope, if we resolve ConsulconDbContext it will be the SAME instance?
                // Wait, scope is created NEW here.
                // But AddDbContext defines connection string based on ICurrentTenantService.
                // In this scope, ICurrentTenantService (if scoped) might be null or empty?
                // Usually CurrentTenantService gets tenant from HttpContext. Here we are in a background task/startup?
                // Initializer is usually run at startup. So no HttpContext. So TenantId is null.
                // So ConsulconDbContext defaults to Master DB.
                
                var masterContext = scope.ServiceProvider.GetRequiredService<ConsulconDbContext>();
                await masterContext.Database.EnsureCreatedAsync();
                
                // Seed Super Admin
                if (!await masterContext.UsuariosMaster.AnyAsync())
                {
                    _logger.LogInformation("Creating default Super Admin user...");
                    var adminUser = new Consulcon.Domain.Entities.Master.UsuarioMaster
                    {
                        Username = "admin",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Email = "admin@consulcon.com",
                        EsSuperAdmin = true,
                        FechaCreacion = DateTime.UtcNow
                    };
                    masterContext.UsuariosMaster.Add(adminUser);
                    await masterContext.SaveChangesAsync();
                    _logger.LogInformation("✅ Super Admin user created (User: admin, Pass: admin123).");
                }
                
                _logger.LogInformation("✅ Master Database initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to initialize Master Database.");
                // We don't throw here to allow app to start even if Master DB logic fails (e.g. connection issue)
                // but usually this is critical. For now, we log and continue to Tenant DB check.
            }

            // 2. Initialize DEFAULT/TENANT Database (Existing Logic)
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ConsulconDbContext>();
                bool canConnect = false;

                try 
                {
                    // Ensure the database is created (EF Core standard)
                    await dbContext.Database.EnsureCreatedAsync(); 
                    canConnect = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Verification: Could not ensure Tenant/Default database created. ContextId: {Context}. Error: {Message}", dbContext.ContextId, ex.Message);
                }

                // Only proceed if we can connect
                if (canConnect && await dbContext.Database.CanConnectAsync()) 
                {
                    await EnsureHistoryTableExists(dbContext);
                    
                    var migrationFolder = Path.Combine(AppContext.BaseDirectory, "migrations"); 
                    
                    if (!Directory.Exists(migrationFolder))
                    {
                        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;
                        if (projectRoot != null)
                        {
                            migrationFolder = Path.Combine(projectRoot, "scripts", "database", "migrations", "active");
                        }
                    }

                    if (Directory.Exists(migrationFolder))
                    {
                        var sqlFiles = Directory.GetFiles(migrationFolder, "*.sql").OrderBy(f => f).ToList();

                        if (sqlFiles.Count > 0)
                        {
                            if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
                            {
                                _logger.LogInformation("Skipping SQL migrations for InMemory database.");
                                return;
                            }

                            using var connection = new MySqlConnection(dbContext.Database.GetConnectionString());

                            await connection.OpenAsync();
                            
                            foreach (var file in sqlFiles)
                            {
                                var fileName = Path.GetFileName(file);
                                
                                var isExecuted = await connection.ExecuteScalarAsync<bool>(
                                    "SELECT COUNT(1) FROM __ExternalMigrationsHistory WHERE MigrationId = @MigrationId", 
                                    new { MigrationId = fileName });

                                if (!isExecuted)
                                {
                                    _logger.LogInformation("🚀 Executing Migration: {FileName}", fileName);
                                    
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
                                        _logger.LogInformation("✅ Migration {FileName} applied successfully.", fileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        await transaction.RollbackAsync();
                                        _logger.LogError(ex, "❌ Error executing migration {FileName}", fileName);
                                        throw; 
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the Tenant/Default database.");
                // Ensure app doesn't crash entirely if just one DB fails
            }
        }

        private static async Task EnsureHistoryTableExists(ConsulconDbContext dbContext)
        {
            if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            {
                return;
            }

            var sql = @"
                CREATE TABLE IF NOT EXISTS `__ExternalMigrationsHistory` (
                    `MigrationId` varchar(150) NOT NULL,
                    `AppliedOn` datetime NOT NULL,
                    PRIMARY KEY (`MigrationId`)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

            await dbContext.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
