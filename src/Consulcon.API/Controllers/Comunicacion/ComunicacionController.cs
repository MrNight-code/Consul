using Consulcon.Application.DTOs.Comunicacion;
using Consulcon.Application.Interfaces.Comunicacion;

namespace Consulcon.API.Controllers.Comunicacion;

[ApiController]
[Route("api/[controller]")]
public class ComunicacionController : ControllerBase
{
    private readonly IComunicacionService _service;

    public ComunicacionController(IComunicacionService service)
    {
        _service = service;
    }

    [HttpGet("condominio/{condominioId}")]
    public async Task<IActionResult> GetComunicados(int condominioId)
    {
        var result = await _service.GetComunicadosByCondominioAsync(condominioId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateComunicado([FromBody] CreateComunicadoDto dto)
    {
        var result = await _service.CreateComunicadoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteComunicadoAsync(id);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
