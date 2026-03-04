using Consulcon.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder; // Nuevo
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace Consulcon.IntegrationTests;

public class ConsulconWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName;

    public ConsulconWebApplicationFactory()
    {
        _dbName = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable("UseInMemoryDatabase", "true");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Mantenemos tus servicios No-Op originales
            services.AddScoped<Consulcon.Application.Interfaces.ITenantDatabaseService, NoOpTenantDatabaseService>();
            services.AddScoped<Consulcon.Application.Interfaces.ITenantMigrationService, NoOpTenantMigrationService>();
            
            // Inyectamos nuestro filtro mágico que saltará la seguridad en los tests
            services.AddTransient<IStartupFilter, AutoAuthStartupFilter>();
        });
    }

    // --- Clases MOCK para DB ---
    private class NoOpTenantDatabaseService : Consulcon.Application.Interfaces.ITenantDatabaseService
    {
        public Task CreateDatabaseAsync(string databaseName) => Task.CompletedTask;
        public Task InitializeDatabaseAsync(string databaseName) => Task.CompletedTask;
        public Task DeleteDatabaseAsync(string databaseName) => Task.CompletedTask;
        public Task InitializeCondominioAsync(string databaseName, Consulcon.Application.DTOs.Inmuebles.CondominioDto initialData) => Task.CompletedTask;
        public Task<Consulcon.Application.DTOs.Inmuebles.CondominioDto?> GetCondominioAsync(string databaseName) => Task.FromResult<Consulcon.Application.DTOs.Inmuebles.CondominioDto?>(null);
    }

    private class NoOpTenantMigrationService : Consulcon.Application.Interfaces.ITenantMigrationService
    {
        public Task MigrateTenantDatabaseAsync(string tenantDbName) => Task.CompletedTask;
    }

    private class AutoAuthStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.Use(async (context, nextMiddleware) =>
                {
                    var claims = new[] 
                    { 
                        new Claim(ClaimTypes.NameIdentifier, "1"), 
                        new Claim(ClaimTypes.Name, "TestUser")
                    };
                    var identity = new ClaimsIdentity(claims, "TestAuth");
                    
                    context.User = new ClaimsPrincipal(identity);

                    await nextMiddleware();
                });

                next(builder);
            };
        }
    }
}