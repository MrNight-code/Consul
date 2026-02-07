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
            // Connect without specifying a database to run CREATE DATABASE
            var dbHost = _configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST configuration is required.");
            var dbPort = _configuration["DB_PORT"] ?? "3306";
            var dbUser = _configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER configuration is required.");
            var dbPassword = _configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("DB_PASSWORD configuration is required.");

            return $"Server={dbHost};Port={dbPort};User={dbUser};Password={dbPassword};";
        }

        private string GetConnectionStringForDatabase(string databaseName)
        {
            var dbHost = _configuration["DB_HOST"] ?? throw new InvalidOperationException("DB_HOST configuration is required.");
            var dbPort = _configuration["DB_PORT"] ?? "3306";
            var dbUser = _configuration["DB_USER"] ?? throw new InvalidOperationException("DB_USER configuration is required.");
            var dbPassword = _configuration["DB_PASSWORD"] ?? throw new InvalidOperationException("DB_PASSWORD configuration is required.");

            /*
                Note: We are manually constructing the connection string here.
                We ensure consistency with DependencyInjection logic but targeted at a specific DB.
            */
            return $"Server={dbHost};Port={dbPort};Database={databaseName};User={dbUser};Password={dbPassword};";
        }

        public async Task InitializeCondominioAsync(string databaseName, Application.DTOs.Inmuebles.CondominioDto initialData)
        {
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

            var tenantCondominio = new Domain.Entities.Inmuebles.Condominio
            {
                IdCondominio = initialData.IdCondominio,
                Nombre = initialData.Nombre,
                Direccion = initialData.Direccion,
                SuperficieTotalM2 = initialData.SuperficieTotalM2,
                IdAdminPersonaNavigation = adminPersona,
                ConfigDiaCobro = initialData.ConfigDiaCobro,
                Logo = initialData.Logo
            };

            context.Condominios.Add(tenantCondominio);
            await context.SaveChangesAsync();
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
