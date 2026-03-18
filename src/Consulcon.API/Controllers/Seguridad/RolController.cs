using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Seguridad;

[ApiController]
[Route("api/[controller]")]
public class RolController : ControllerBase
{
    private readonly IRolService _service;

    public RolController(IRolService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
