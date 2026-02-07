using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Interfaces.Contabilidad;

public interface IProveedorService
{
    // Métodos legacy (mantener compatibilidad)
    Task<Result<IEnumerable<ProveedorDto>>> GetAllAsync();
    Task<Result<ProveedorDto>> GetByIdAsync(int id);
    Task<Result<ProveedorDto>> CreateAsync(ProveedorDto dto);
    Task<Result<ProveedorDto>> UpdateAsync(int id, ProveedorDto dto);
    Task<Result<bool>> DeleteAsync(int id);

    /// Obtiene proveedores paginados con búsqueda opcional
    Task<Result<PagedResult<ProviderDto>>> GetPagedAsync(
        int page = 1,
        int pageSize = 20,
        string? term = null,
        CancellationToken cancellationToken = default);

    /// Obtiene un proveedor por ID (nuevo DTO)
    Task<Result<ProviderDto>> GetProviderByIdAsync(int id, CancellationToken cancellationToken = default);

    /// Crea un nuevo proveedor y retorna su ID
    Task<Result<int>> CreateProviderAsync(CreateProviderDto dto, CancellationToken cancellationToken = default);

    /// Actualiza un proveedor existente
    Task<Result> UpdateProviderAsync(int id, UpdateProviderDto dto, CancellationToken cancellationToken = default);

    /// Elimina (soft delete) un proveedor
    Task<Result> DeleteProviderAsync(int id, CancellationToken cancellationToken = default);
}
