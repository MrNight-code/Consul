using Consulcon.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Threading.Tasks;

namespace Consulcon.Infrastructure.Persistence.Services
{
    public class TenantDatabaseService(IConfiguration configuration) : ITenantDatabaseService
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task CreateDatabaseAsync(string databaseName)
        {
            var masterConnectionString = GetMasterConnectionString();
            
            using var connection = new MySqlConnection(masterConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            // Create database if it doesn't exist
            command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
            await command.ExecuteNonQueryAsync();
        }

        public async Task InitializeDatabaseAsync(string databaseName)
        {
            var connectionString = GetConnectionStringForDatabase(databaseName);
            
            var optionsBuilder = new DbContextOptionsBuilder<ConsulconDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            using var context = new ConsulconDbContext(optionsBuilder.Options);
            await context.Database.EnsureCreatedAsync();
        }

        public async Task DeleteDatabaseAsync(string databaseName)
        {
            var masterConnectionString = GetMasterConnectionString();
            
            using var connection = new MySqlConnection(masterConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            // Drop database if it exists - USE WITH CAUTION
            command.CommandText = $"DROP DATABASE IF EXISTS `{databaseName}`;";
            await command.ExecuteNonQueryAsync();
        }

        private string GetMasterConnectionString()
        {
            // Use environment variables first (Docker), fallback to configuration (local dev)
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? _configuration["DB_HOST"];
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? _configuration["DB_PORT"] ?? "3306";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? _configuration["DB_USER"];
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? _configuration["DB_PASSWORD"];

            if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
            {
                throw new InvalidOperationException("Database configuration is required. Set DB_HOST, DB_USER, DB_PASSWORD.");
            }

            return $"Server={dbHost};Port={dbPort};User={dbUser};Password={dbPassword};";
        }

        private string GetConnectionStringForDatabase(string databaseName)
        {
            // Use environment variables first (Docker), fallback to configuration (local dev)
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? _configuration["DB_HOST"];
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? _configuration["DB_PORT"] ?? "3306";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? _configuration["DB_USER"];
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? _configuration["DB_PASSWORD"];

            if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
            {
                throw new InvalidOperationException("Database configuration is required. Set DB_HOST, DB_USER, DB_PASSWORD.");
            }

            return $"Server={dbHost};Port={dbPort};Database={databaseName};User={dbUser};Password={dbPassword};";
        }

        public async Task InitializeCondominioAsync(string databaseName, Application.DTOs.Inmuebles.CondominioDto initialData)
        {
            try
            {
                Console.WriteLine($"[TenantDatabaseService] Initializing condominio in {databaseName}");
                var connectionString = GetConnectionStringForDatabase(databaseName);
                
                var optionsBuilder = new DbContextOptionsBuilder<ConsulconDbContext>();
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                using var context = new ConsulconDbContext(optionsBuilder.Options); 
                
                // Create initial Admin Persona to satisfy FK constraint
                var adminPersona = new Domain.Entities.General.Persona
                {
                    NombreCompleto = !string.IsNullOrEmpty(initialData.AdminNombre) ? initialData.AdminNombre : "Administrador Inicial",
                    EsActivo = true
                };

                // First save the Persona to get its ID
                context.Personas.Add(adminPersona);
                await context.SaveChangesAsync();
                Console.WriteLine($"[TenantDatabaseService] Created admin persona with ID: {adminPersona.IdPersona}");

                var tenantCondominio = new Domain.Entities.Inmuebles.Condominio
                {
                    // Don't set IdCondominio - let DB auto-generate it
                    Nombre = initialData.Nombre,
                    Direccion = initialData.Direccion,
                    SuperficieTotalM2 = initialData.SuperficieTotalM2,
                    IdAdminPersona = adminPersona.IdPersona,
                    ConfigDiaCobro = initialData.ConfigDiaCobro,
                    Logo = initialData.Logo
                };

                context.Condominios.Add(tenantCondominio);
                await context.SaveChangesAsync();
                Console.WriteLine($"[TenantDatabaseService] Created tenant condominio with ID: {tenantCondominio.IdCondominio}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TenantDatabaseService] ERROR initializing condominio: {ex.Message}");
                Console.WriteLine($"[TenantDatabaseService] Stack: {ex.StackTrace}");
                throw; // Re-throw to let caller handle
            }
        }

        public async Task<Application.DTOs.Inmuebles.CondominioDto?> GetCondominioAsync(string databaseName)
        {
            try
            {
                var connectionString = GetConnectionStringForDatabase(databaseName);
                
                var optionsBuilder = new DbContextOptionsBuilder<ConsulconDbContext>();
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                using var context = new ConsulconDbContext(optionsBuilder.Options);
                
                var condo = await context.Condominios
                    .Include(c => c.IdAdminPersonaNavigation)
                    .FirstOrDefaultAsync();

                if (condo == null) return null;

                return new Application.DTOs.Inmuebles.CondominioDto
                {
                    IdCondominio = condo.IdCondominio,
                    Nombre = condo.Nombre,
                    Direccion = condo.Direccion,
                    SuperficieTotalM2 = condo.SuperficieTotalM2,
                    IdAdminPersona = condo.IdAdminPersona,
                    AdminNombre = condo.IdAdminPersonaNavigation?.NombreCompleto,
                    ConfigDiaCobro = condo.ConfigDiaCobro,
                    Logo = condo.Logo
                };
            }
            catch (MySqlConnector.MySqlException)
            {
                // Tenant database doesn't exist yet - return null to fallback to Master data
                return null;
            }
        }
    }
}
