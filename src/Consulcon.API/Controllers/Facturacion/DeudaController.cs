using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;

namespace Consulcon.API.Controllers.Facturacion;

[ApiController]
[Route("api/[controller]")]
public class DeudaController : ControllerBase
{
    private readonly IDeudaService _service;

    public DeudaController(IDeudaService service)
    {
        _service = service;
    }

    [HttpGet("pendiente")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _service.GetPendingAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("contrato/{contratoId}")]
    public async Task<IActionResult> GetByContrato(int contratoId)
    {
        var result = await _service.GetByContratoAsync(contratoId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { Message = result.Error });
    }

    [HttpPost("generar")]
    public async Task<IActionResult> Generate([FromBody] GenerateDeudaDto dto)
    {
        var result = await _service.GenerateDeudaAsync(dto);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value) : BadRequest(result.Error);
    }
}
