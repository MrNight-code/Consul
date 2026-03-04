using Consulcon.Application.DTOs;

namespace Consulcon.Application.Interfaces.Inmuebles;

public interface IPropiedadService
{
    Task<Result<IEnumerable<PropiedadDto>>> GetAllAsync(string[]? expand = null);
    Task<Result<IEnumerable<PropiedadDto>>> GetByCondominioAsync(int condominioId, string[]? expand = null);
    Task<Result<PropiedadDto>> GetByIdAsync(int id, string[]? expand = null);
    Task<Result<PropiedadDto>> CreateAsync(CreatePropiedadDto dto);
    Task<Result<PropiedadDto>> UpdateAsync(int id, CreatePropiedadDto dto);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<PagedResult<PropiedadDto>>> GetPagedAsync(int idCondominio, PaginationParams parameters);
}
