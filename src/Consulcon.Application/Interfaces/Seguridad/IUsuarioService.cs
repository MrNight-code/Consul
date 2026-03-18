using Consulcon.Application.DTOs.Seguridad;

namespace Consulcon.Application.Interfaces.Seguridad;

public interface IUsuarioService
{
    Task<Result<IEnumerable<UserDto>>> GetAllAsync();
    Task<Result<UserDto>> GetByIdAsync(int id);
    Task<Result<UserDto>> CreateAsync(CreateUserDto dto);
    Task<Result<UserDto>> UpdateAsync(int id, UpdateUserDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}
