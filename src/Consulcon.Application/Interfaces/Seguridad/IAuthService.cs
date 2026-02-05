using Consulcon.Application.DTOs.Seguridad;

namespace Consulcon.Application.Interfaces.Seguridad;

public interface IAuthService
{
    Task<Result<UserDto>> LoginAsync(string username, string password);
}
