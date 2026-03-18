using Consulcon.Application.DTOs.Seguridad;

namespace Consulcon.Application.Interfaces.Seguridad;

public interface IMasterIdentityService
{
    Task<(int? UserId, string? Username, string? Email, bool? EsSuperAdmin, List<TenantDto>? Tenants)> ValidateUserAsync(string username, string password);
}

public class TenantDto
{
    public int Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
