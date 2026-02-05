using Consulcon.Application.DTOs.Contratos;
using Consulcon.Application.Interfaces.Contratos;

namespace Consulcon.API.Controllers.Contratos;

[ApiController]
[Route("api/[controller]")]
public class ContratoController : ControllerBase
{
    private readonly IContratoService _service;

    public ContratoController(IContratoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("propiedad/{propiedadId}")]
    public async Task<IActionResult> GetByPropiedad(int propiedadId)
    {
        var result = await _service.GetByPropiedadAsync(propiedadId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { Message = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContratoDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/participante")]
    public async Task<IActionResult> AddParticipante(int id, [FromBody] CreateContratoParticipanteDto dto)
    {
        var result = await _service.AddParticipanteAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}/finalizar")]
    public async Task<IActionResult> Terminate(int id, [FromBody] TerminateRequest request) 
    {
        var result = await _service.TerminateAsync(id, request.Motivo, request.FechaFin);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}

public class TerminateRequest
{
    public string Motivo { get; set; } = string.Empty;
    public DateOnly FechaFin { get; set; }
}
