namespace Consulcon.Domain.Interfaces;

public interface ICurrentTenantService
{
    string? TenantId { get; }
}
