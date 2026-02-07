using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.General;
using Consulcon.Domain.Interfaces;

namespace Consulcon.Application.Interfaces.Contabilidad;
/// Repositorio especializado para Provider con operaciones de paginación y búsqueda
public interface IProviderRepository : IRepository<Proveedor>
{
    /// Obtiene proveedores paginados con búsqueda opcional
    Task<PagedResult<Proveedor>> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default);

    /// Verifica si existe un proveedor con el NIT especificado
    Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);

    /// Verifica si existe un proveedor con el NIT especificado, excluyendo un ID
    Task<bool> ExistsByTaxIdAsync(string taxId, int excludeId, CancellationToken cancellationToken = default);
}
