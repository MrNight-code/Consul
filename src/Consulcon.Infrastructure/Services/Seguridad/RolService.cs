using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Consulcon.Domain.Common;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Consulcon.Infrastructure.Services.Seguridad;

public class RolService(IConfiguration configuration) : IRolService
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<Result<IEnumerable<RolDto>>> GetAllAsync()
    {
        try
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? _configuration["DB_HOST"];
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? _configuration["DB_PORT"] ?? "3306";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? _configuration["DB_USER"];
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? _configuration["DB_PASSWORD"];
            var dbName = "db_consulcon_master"; // explicitly querying master database

            if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
            {
                return Result.Fail<IEnumerable<RolDto>>("Database configuration is required. Set DB_HOST, DB_USER, DB_PASSWORD.");
            }

            var connString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";

            using var connection = new MySqlConnection(connString);
            await connection.OpenAsync();

            var roles = await connection.QueryAsync<RolDto>("SELECT IdRol, Nombre FROM RolesMaster");
            
            return Result.Ok(roles);
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<RolDto>>($"Error al conectar a la base de datos Master: {ex.Message}");
        }
    }
}
