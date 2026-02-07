namespace Consulcon.Application.Interfaces;

public interface ITenantDatabaseService
{
    Task CreateDatabaseAsync(string databaseName);
    Task InitializeDatabaseAsync(string databaseName);
    Task DeleteDatabaseAsync(string databaseName);
    Task InitializeCondominioAsync(string databaseName, DTOs.Inmuebles.CondominioDto initialData);
    Task<DTOs.Inmuebles.CondominioDto?> GetCondominioAsync(string databaseName);
}

