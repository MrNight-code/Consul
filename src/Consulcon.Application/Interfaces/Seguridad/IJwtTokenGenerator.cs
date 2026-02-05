using Consulcon.Application.DTOs.Seguridad;

namespace Consulcon.Application.Interfaces.Seguridad;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserDto user);
}
