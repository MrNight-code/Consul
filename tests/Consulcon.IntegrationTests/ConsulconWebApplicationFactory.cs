using Consulcon.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Consulcon.IntegrationTests;

public class ConsulconWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName;

    public ConsulconWebApplicationFactory()
    {
        _dbName = Guid.NewGuid().ToString();
        // Set environment variable to ensure Program.cs sees it immediately upon CreateBuilder
        Environment.SetEnvironmentVariable("UseInMemoryDatabase", "true");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace ITenantDatabaseService and ITenantMigrationService with No-Op for tests
            // to avoid trying to create real MySQL databases during In-Memory tests.
            services.AddScoped<Consulcon.Application.Interfaces.ITenantDatabaseService, NoOpTenantDatabaseService>();
            services.AddScoped<Consulcon.Application.Interfaces.ITenantMigrationService, NoOpTenantMigrationService>();
        });
    }

    private class NoOpTenantDatabaseService : Consulcon.Application.Interfaces.ITenantDatabaseService
    {
        public Task CreateDatabaseAsync(string databaseName) => Task.CompletedTask;
        public Task InitializeDatabaseAsync(string databaseName) => Task.CompletedTask;
        public Task DeleteDatabaseAsync(string databaseName) => Task.CompletedTask;
    }

    private class NoOpTenantMigrationService : Consulcon.Application.Interfaces.ITenantMigrationService
    {
        public Task MigrateTenantDatabaseAsync(string tenantDbName) => Task.CompletedTask;
    }
}
