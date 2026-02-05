using Consulcon.Application.DTOs.Contabilidad;

namespace Consulcon.Application.Interfaces.Contabilidad;

public interface ITesoreriaService
{
    Task<Result<IEnumerable<BancoDto>>> GetBancosAsync();
    Task<Result<IEnumerable<FormaPagoDto>>> GetFormasPagoAsync();
    Task<Result<BancoDto>> CreateBancoAsync(BancoDto dto);
    Task<Result<FormaPagoDto>> CreateFormaPagoAsync(FormaPagoDto dto);
    
    // Egresos
    Task<Result<IEnumerable<EgresoDto>>> GetEgresosByCondominioAsync(int condominioId);
    Task<Result<EgresoDto>> RegistrarEgresoAsync(CreateEgresoDto dto);
}
