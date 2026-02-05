using Consulcon.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Consulcon.API.Services;

public class CurrentTenantService(IHttpContextAccessor httpContextAccessor) : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? TenantId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantId))
                {
                    Console.WriteLine($"[CurrentTenantService] Found TenantId header: {tenantId}");
                    return tenantId.ToString();
                }
                else
                {
                    Console.WriteLine("[CurrentTenantService] X-Tenant-Id header NOT found in request.");
                }
            }
            else
            {
                Console.WriteLine("[CurrentTenantService] HttpContext is null.");
            }
            return null;
        }
    }
}
