namespace Consulcon.Domain.Interfaces;

public interface ICurrentTenantService
{
    int? CondominioId { get; }
    string? TenantId { get; }
    
    /// <summary>
    /// True if a X-Condominio-Id header was provided but the condominio doesn't exist
    /// </summary>
    bool TenantResolutionFailed { get; }
    
    /// <summary>
    /// Error message when TenantResolutionFailed is true
    /// </summary>
    string? TenantResolutionError { get; }
}
