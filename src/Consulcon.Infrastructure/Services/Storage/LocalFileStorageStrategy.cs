using System.IO;
using System.Threading.Tasks;
using System;

namespace Consulcon.Infrastructure.Services.Storage;

public interface IFileStorageStrategy
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, int tenantId, int year);
    Task<Stream> GetFileAsync(string storagePath);
    Task DeleteFileAsync(string storagePath);
}

public class LocalFileStorageStrategy(string basePath = "App_Data") : IFileStorageStrategy
{
    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, int tenantId, int year)
    {
        // Path: App_Data/Tenants/{TenantId}/Expenses/{Year}/{Guid}.ext
        // We assume fileName is already the stored filename (Guid.ext)
        
        var relativeFolder = Path.Combine("Tenants", tenantId.ToString(), "Expenses", year.ToString());
        var fullFolder = Path.Combine(basePath, relativeFolder);

        if (!Directory.Exists(fullFolder))
        {
            Directory.CreateDirectory(fullFolder);
        }

        var fullPath = Path.Combine(fullFolder, fileName);
        var relativePath = Path.Combine(relativeFolder, fileName);

        using (var file = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(file);
        }

        return relativePath;
    }

    public Task<Stream> GetFileAsync(string storagePath)
    {
        var fullPath = Path.Combine(basePath, storagePath);
        
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("El archivo no existe en el almacenamiento.", fullPath);
        }

        // Return stream, let caller dispose
        return Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
    }

    public Task DeleteFileAsync(string storagePath)
    {
        var fullPath = Path.Combine(basePath, storagePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }
}
