using Consulcon.Application.DTOs.Contratos;

namespace Consulcon.Application.Interfaces.Contratos;

public interface IContratoService
{
    Task<Result<IEnumerable<ContratoDto>>> GetAllAsync();
    Task<Result<IEnumerable<ContratoDto>>> GetByPropiedadAsync(int propiedadId);
    Task<Result<ContratoDto>> GetByIdAsync(int id);
    Task<Result<ContratoDto>> CreateAsync(CreateContratoDto dto);
    Task<Result<ContratoDto>> AddParticipanteAsync(int contratoId, CreateContratoParticipanteDto dto);
    Task<Result<bool>> TerminateAsync(int id, string motivo, DateOnly fechaFin);
}
