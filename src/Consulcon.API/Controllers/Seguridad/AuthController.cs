using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Seguridad;

public class AuthController(IAuthService authService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request.Username, request.Password);

        if (result.IsFailure)
            return Unauthorized(new { message = result.Error });

        return Ok(result.Value);
    }
}