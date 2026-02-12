using Consulcon.Domain.Interfaces;
using Consulcon.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Consulcon.API.Services;

public class CurrentTenantService : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private int? _condominioId;
    private string? _tenantId;
    private bool _resolved = false;
    private bool _tenantResolutionFailed = false;
    private string? _tenantResolutionError;

    public CurrentTenantService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public int? CondominioId
    {
        get
        {
            ResolveIfNeeded();
            return _condominioId;
        }
    }

    public string? TenantId
    {
        get
        {
            ResolveIfNeeded();
            return _tenantId;
        }
    }

    public bool TenantResolutionFailed
    {
        get
        {
            ResolveIfNeeded();
            return _tenantResolutionFailed;
        }
    }

    public string? TenantResolutionError
    {
        get
        {
            ResolveIfNeeded();
            return _tenantResolutionError;
        }
    }

    private void ResolveIfNeeded()
    {
        if (_resolved) return;
        _resolved = true;

        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            Console.WriteLine("[CurrentTenantService] HttpContext is null.");
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Condominio-Id", out var condominioIdHeader))
        {
            Console.WriteLine("[CurrentTenantService] X-Condominio-Id header NOT found in request.");
            return;
        }

        if (!int.TryParse(condominioIdHeader.ToString(), out int condominioId))
        {
            Console.WriteLine($"[CurrentTenantService] Invalid X-Condominio-Id: {condominioIdHeader}");
            return;
        }

        _condominioId = condominioId;
        Console.WriteLine($"[CurrentTenantService] Found CondominioId header: {condominioId}");

        // Resolve TenantId from Master DB
        try
        {
            // Build connection string using same logic as DependencyInjection.cs
            var defaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
            
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbName = _configuration["DB_MASTER_NAME"] ?? "db_consulcon_master";
            
            // Fallback to appsettings if env vars not set
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
            }
            
            if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
            {
                Console.WriteLine("[CurrentTenantService] DB connection not configured.");
                return;
            }
            
            var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";
            
            var optionsBuilder = new DbContextOptionsBuilder<ConsulconDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            using var masterContext = new ConsulconDbContext(optionsBuilder.Options);
            var condominio = masterContext.CondominiosMaster
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == condominioId);

            if (condominio != null)
            {
                _tenantId = condominio.TenantId;
                Console.WriteLine($"[CurrentTenantService] Resolved TenantId: {_tenantId}");
            }
            else
            {
                _tenantResolutionFailed = true;
                _tenantResolutionError = $"Condominio {condominioId} no existe o fue eliminado.";
                Console.WriteLine($"[CurrentTenantService] Condominio {condominioId} not found in Master.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CurrentTenantService] Error resolving TenantId: {ex.Message}");
        }
    }
}
