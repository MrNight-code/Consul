using Consulcon.Application.DTOs.Facturacion;

namespace Consulcon.Application.Interfaces.Facturacion;

public interface IDeudaService
{
    Task<Result<IEnumerable<DeudaDto>>> GetByContratoAsync(int contratoId);
    Task<Result<IEnumerable<DeudaDto>>> GetPendingAsync(); 
    Task<Result<DeudaDto>> GenerateDeudaAsync(GenerateDeudaDto dto);
    Task<Result<DeudaDto>> GetByIdAsync(int id);
    Task<Result<Consulcon.Application.DTOs.Expensas.EstadoCuentaUnidadResponseDto>> GetEstadoCuentaByPropiedadAsync(int propiedadId);
}
