using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Consulcon.Infrastructure.Persistence;
using Consulcon.Infrastructure.Persistence.Repositories;
using Consulcon.Infrastructure.Persistence.Services;
using Consulcon.Domain.Interfaces;
using Consulcon.Application.Interfaces;
using MySqlConnector;

namespace Consulcon.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ConsulconDbContext>(options =>
                    options.UseInMemoryDatabase("ConsulconDb"));
            }
            else
            {
                // Dynamic Connection String Construction
                services.AddDbContext<ConsulconDbContext>((serviceProvider, options) =>
                {
                    var config = serviceProvider.GetRequiredService<IConfiguration>();
                    var tenantService = serviceProvider.GetService<ICurrentTenantService>();
                    var defaultConnectionString = config.GetConnectionString("DefaultConnection");
                    
                    // Read from environment variables (required in production)
                    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
                    var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
                    var dbUser = Environment.GetEnvironmentVariable("DB_USER");
                    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
                    var envName = Environment.GetEnvironmentVariable("DB_NAME");
                    
                    // Fallback: Parse from DefaultConnection if env vars not set (local dev with appsettings)
                    if (string.IsNullOrEmpty(dbHost) && !string.IsNullOrEmpty(defaultConnectionString))
                    {
                        var parts = defaultConnectionString.Split(';')
                            .Select(p => p.Trim())
                            .Where(p => !string.IsNullOrEmpty(p))
                            .Select(p => p.Split('=', 2))
                            .Where(p => p.Length == 2)
                            .ToDictionary(p => p[0].Trim(), p => p[1].Trim(), StringComparer.OrdinalIgnoreCase);
                        
                        if (parts.TryGetValue("Server", out var server)) dbHost = server;
                        if (parts.TryGetValue("Port", out var port)) dbPort = port;
                        if (parts.TryGetValue("User", out var user)) dbUser = user;
                        if (parts.TryGetValue("Password", out var password)) dbPassword = password;
                        if (parts.TryGetValue("Database", out var database) && string.IsNullOrEmpty(envName)) envName = database;
                    }
                    
                    // Validate required configuration
                    if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
                    {
                        throw new InvalidOperationException(
                            "Database configuration is missing. Set environment variables: DB_HOST, DB_USER, DB_PASSWORD " +
                            "(or provide a valid ConnectionStrings:DefaultConnection in appsettings.json for local development).");
                    }
                    
                    // Default port if not specified
                    if (string.IsNullOrEmpty(dbPort)) dbPort = "3306";
                    
                    // Determine Database Name
                    string? dbName = null;
                    if (tenantService?.TenantId != null && !string.IsNullOrEmpty(tenantService.TenantId))
                    {
                        dbName = tenantService.TenantId.StartsWith("db_") ? tenantService.TenantId : $"db_condominio_{tenantService.TenantId}";
                    }
                    
                    if (string.IsNullOrEmpty(dbName) && !string.IsNullOrEmpty(envName))
                    {
                        dbName = envName;
                    }

                    // Default to Master DB if no tenant and no env override
                    if (string.IsNullOrEmpty(dbName))
                    {
                        dbName = config["DB_MASTER_NAME"] ?? "db_consulcon_master";
                    }

                    // Build Connection String
                    string connectionString;
                    if (!string.IsNullOrEmpty(defaultConnectionString))
                    {
                        // Use existing connection string as base to preserve other parameters
                        var builder = new MySqlConnector.MySqlConnectionStringBuilder(defaultConnectionString)
                        {
                            Server = dbHost,
                            UserID = dbUser,
                            Password = dbPassword
                        };
                        
                        if (uint.TryParse(dbPort, out uint port)) builder.Port = port;
                        
                        // Always override database if we determined one
                        if (!string.IsNullOrEmpty(dbName))
                        {
                            builder.Database = dbName;
                        }

                        connectionString = builder.ConnectionString;
                    }
                    else
                    {
                        connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";
                    }

                    // Debug Log for troubleshooting connection issues
                    Console.WriteLine($"[DI] Configuring DB Connection: Host={dbHost}, Port={dbPort}, DB={dbName}, User={dbUser}");

                    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)));
                    // If nothing configured, EF will throw a helpful error
                });
            }

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            
            // Specialized Repositories
            services.AddScoped<Consulcon.Application.Interfaces.Contabilidad.IProviderRepository, Consulcon.Infrastructure.Persistence.Repositories.ProviderRepository>();
            
            services.AddScoped<DatabaseMigrationInitializer>();
            services.AddScoped<ITenantDatabaseService, TenantDatabaseService>();
            services.AddScoped<ITenantMigrationService, TenantMigrationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // PDF Generators
            services.AddScoped<Consulcon.Application.Interfaces.Facturacion.IReceiptGenerationService, Services.Facturacion.ReceiptGenerationService>();

            // Cobranzas (Transactions)
            services.AddScoped<ICobranzaService, Services.CobranzaService>();

            // Accounts (Configuration)
            services.AddScoped<IAccountService, Services.AccountService>();

            // Financial Config
            services.AddScoped<IFinancialConfigService, Services.FinancialConfigService>();

            // Ownership (Property Assignment & History)
            services.AddScoped<Consulcon.Application.Interfaces.Inmuebles.IOwnershipService, Services.Inmuebles.OwnershipService>();

            services.AddScoped<Consulcon.Application.Interfaces.Seguridad.IMasterIdentityService, Consulcon.Infrastructure.Services.MasterIdentityService>();

            // Dashboard Metrics
            services.AddScoped<Consulcon.Application.Interfaces.Dashboard.IDashboardMetricsService, Consulcon.Application.Services.Dashboard.DashboardMetricsService>();

            // Universal Padron API (Personas & Propiedades)
            services.AddScoped<Consulcon.Application.Interfaces.Personas.IPersonaService, Consulcon.Application.Services.Personas.PersonaService>();
            services.AddScoped<Consulcon.Application.Interfaces.Inmuebles.IPropiedadService, Consulcon.Application.Services.Inmuebles.PropiedadService>();

            services.AddScoped<Consulcon.Application.Interfaces.Inmuebles.IPropiedadService, Consulcon.Application.Services.Inmuebles.PropiedadService>();

            // Expense Attachments
            services.AddScoped<Consulcon.Infrastructure.Services.Storage.IFileStorageStrategy, Consulcon.Infrastructure.Services.Storage.LocalFileStorageStrategy>();
            services.AddScoped<Consulcon.Application.Interfaces.Contabilidad.IExpenseAttachmentService, Consulcon.Infrastructure.Services.Contabilidad.ExpenseAttachmentService>();
            
            // Expenses (Registration)
            services.AddScoped<Consulcon.Application.Interfaces.Contabilidad.IExpenseService, Consulcon.Infrastructure.Services.Contabilidad.ExpenseService>();
            
            // Expense Receipts (PDF Generation)
            services.AddScoped<Consulcon.Application.Interfaces.Facturacion.IExpenseReceiptGenerationService, Consulcon.Infrastructure.Services.Facturacion.ExpenseReceiptGenerationService>();
            
            // Cash Book (Libro de Caja)
            services.AddScoped<Consulcon.Application.Interfaces.Contabilidad.ICashBookService, Consulcon.Infrastructure.Services.Contabilidad.CashBookService>();
            
            return services;
        }
    }
}
