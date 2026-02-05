using Consulcon.Application.DTOs.Personas; // Correct namespace

namespace Consulcon.Application.Interfaces.Personas;

public interface IPersonaService
{
    Task<Result<IEnumerable<PersonaDto>>> GetAllAsync();
    Task<Result<PersonaDto>> GetByIdAsync(int id);
    Task<Result<PersonaDto>> CreateAsync(PersonaDto dto);
    Task<Result<PersonaDto>> UpdateAsync(int id, PersonaDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}
