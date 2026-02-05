using Consulcon.Application.DTOs.Contratos;

namespace Consulcon.Application.Interfaces.Contratos;

public interface ICatalogoServicioService
{
    Task<Result<IEnumerable<CatalogoServicioDto>>> GetAllAsync();
    Task<Result<CatalogoServicioDto>> CreateAsync(CatalogoServicioDto dto);
    Task<Result<CatalogoServicioDto>> UpdateAsync(int id, CatalogoServicioDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}
