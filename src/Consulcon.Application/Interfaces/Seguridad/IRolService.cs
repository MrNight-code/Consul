using Consulcon.Application.DTOs.Seguridad;

namespace Consulcon.Application.Interfaces.Seguridad;

public interface IRolService
{
    Task<Result<IEnumerable<RolDto>>> GetAllAsync();
}
