using Consulcon.Application.DTOs.Comunicacion;

namespace Consulcon.Application.Interfaces.Comunicacion;

public interface IComunicacionService
{
    Task<Result<IEnumerable<ComunicadoBlogDto>>> GetComunicadosByCondominioAsync(int condominioId);
    Task<Result<ComunicadoBlogDto>> CreateComunicadoAsync(CreateComunicadoDto dto);
    Task<Result<bool>> DeleteComunicadoAsync(int id);
}
