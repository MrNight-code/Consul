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
            var dbHost = _configuration["DB_HOST"];
            var dbPort = _configuration["DB_PORT"] ?? "3306";
            var dbUser = _configuration["DB_USER"];
            var dbPassword = _configuration["DB_PASSWORD"];

            return $"Server={dbHost};Port={dbPort};User={dbUser};Password={dbPassword};";
        }

        private string GetConnectionStringForDatabase(string databaseName)
        {
            var dbHost = _configuration["DB_HOST"];
            var dbPort = _configuration["DB_PORT"] ?? "3306";
            var dbUser = _configuration["DB_USER"];
            var dbPassword = _configuration["DB_PASSWORD"];

            /*
                Note: We are manually constructing the connection string here.
                We ensure consistency with DependencyInjection logic but targeted at a specific DB.
            */
            return $"Server={dbHost};Port={dbPort};Database={databaseName};User={dbUser};Password={dbPassword};";
        }
    }
}
