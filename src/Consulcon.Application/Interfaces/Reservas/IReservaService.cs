using Consulcon.Application.DTOs.Reservas;

namespace Consulcon.Application.Interfaces.Reservas;

public interface IReservaService
{
    Task<Result<IEnumerable<RecursoComunDto>>> GetRecursosByCondominioAsync(int condominioId);
    Task<Result<RecursoComunDto>> CreateRecursoAsync(RecursoComunDto dto);
    
    Task<Result<IEnumerable<ReservaDto>>> GetReservasByCondominioAsync(int condominioId);
    Task<Result<ReservaDto>> CreateReservaAsync(CreateReservaDto dto);
    Task<Result<bool>> ConfirmarReservaAsync(int id);
    Task<Result<bool>> CancelarReservaAsync(int id);
}
