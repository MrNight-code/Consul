using Consulcon.Application.DTOs.Facturacion;

namespace Consulcon.Application.Interfaces.Facturacion;

public interface IPagoService
{
    Task<Result<IEnumerable<TransaccionPagoDto>>> GetByDeudaAsync(int deudaId);
    Task<Result<TransaccionPagoDto>> RegistrarPagoAsync(CreatePagoDto dto);
}
