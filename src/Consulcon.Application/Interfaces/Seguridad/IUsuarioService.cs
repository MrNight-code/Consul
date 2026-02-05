using Consulcon.Application.DTOs.Seguridad;

namespace Consulcon.Application.Interfaces.Seguridad;

public interface IUsuarioService
{
    Task<Result<IEnumerable<UserDto>>> GetAllAsync();
    Task<Result<UserDto>> GetByIdAsync(int id);
    Task<Result<UserDto>> CreateAsync(CreateUserDto dto);
             // Update logic omitted for brevity but recommended
    Task<Result<bool>> DeleteAsync(int id);
}
