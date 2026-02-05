namespace Consulcon.Application.Interfaces;

public interface ITenantMigrationService
{
    Task MigrateTenantDatabaseAsync(string tenantDbName);
}
