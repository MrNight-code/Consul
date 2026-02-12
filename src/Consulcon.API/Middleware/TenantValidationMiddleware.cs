using Consulcon.Domain.Interfaces;
using System.Text.Json;

namespace Consulcon.API.Middleware;

/// <summary>
/// Middleware that validates tenant resolution before processing requests.
/// Returns 400 Bad Request if X-Condominio-Id header is provided but the condominio doesn't exist.
/// </summary>
public class TenantValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService)
    {
        // Check if tenant resolution failed (header provided but condominio not found)
        if (tenantService.TenantResolutionFailed)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            
            var errorResponse = new
            {
                isSuccess = false,
                isFailure = true,
                errorCode = "ERR-TENANT-404",
                message = tenantService.TenantResolutionError ?? "El condominio especificado no existe.",
                traceId = context.TraceIdentifier,
                timestamp = DateTime.UtcNow
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            return;
        }
        
        await next(context);
    }
}

public static class TenantValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantValidationMiddleware>();
    }
}
