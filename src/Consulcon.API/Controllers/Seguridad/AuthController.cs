using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;

namespace Consulcon.API.Controllers.Seguridad;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Username, request.Password);

        if (result.IsFailure)
        {
            return Unauthorized(new { Message = result.Error });
        }

        return Ok(new 
        { 
            Message = "Login exitoso", 
            Data = result.Value
        });
    }
}
