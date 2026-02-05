using Consulcon.Application.DTOs.Reservas;
using Consulcon.Application.Interfaces.Reservas;

namespace Consulcon.API.Controllers.Reservas;

[ApiController]
[Route("api/[controller]")]
public class ReservaController : ControllerBase
{
    private readonly IReservaService _service;

    public ReservaController(IReservaService service)
    {
        _service = service;
    }

    [HttpGet("recursos/condominio/{condominioId}")]
    public async Task<IActionResult> GetRecursos(int condominioId)
    {
        var result = await _service.GetRecursosByCondominioAsync(condominioId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("recursos")]
    public async Task<IActionResult> CreateRecurso([FromBody] RecursoComunDto dto)
    {
        var result = await _service.CreateRecursoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("condominio/{condominioId}")]
    public async Task<IActionResult> GetReservas(int condominioId)
    {
        var result = await _service.GetReservasByCondominioAsync(condominioId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReserva([FromBody] CreateReservaDto dto)
    {
        var result = await _service.CreateReservaAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}/confirmar")]
    public async Task<IActionResult> Confirmar(int id)
    {
        var result = await _service.ConfirmarReservaAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
