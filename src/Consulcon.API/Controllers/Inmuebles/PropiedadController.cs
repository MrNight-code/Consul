using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;

namespace Consulcon.API.Controllers.Inmuebles;

[ApiController]
[Route("api/[controller]")]
public class PropiedadController(IPropiedadService service) : ControllerBase
{
    private readonly IPropiedadService _service = service;
    private static readonly string[] OwnerExpand = ["owner"];

    /// <summary>
    /// Obtiene todas las propiedades. Use expand=owner para incluir propietarios.
    /// </summary>
    /// <param name="expand">Campos a expandir (ej: "owner")</param>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? expand = null)
    {
        var expandFields = ParseExpandParameter(expand);
        var result = await _service.GetAllAsync(expandFields);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// Obtiene propiedades por condominio. Use expand=owner para incluir propietarios.
    [HttpGet("condominio/{condominioId}")]
    public async Task<IActionResult> GetByCondominio(int condominioId, [FromQuery] string? expand = null)
    {
        var expandFields = ParseExpandParameter(expand);
        var result = await _service.GetByCondominioAsync(condominioId, expandFields);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// Endpoint de conveniencia: Obtiene propiedades por condominio con propietarios incluidos.
    [HttpGet("condominio/{condominioId}/with-owners")]
    public async Task<IActionResult> GetByCondominioWithOwners(int condominioId)
    {
        // Alias conveniente que siempre incluye owner
        var result = await _service.GetByCondominioAsync(condominioId, OwnerExpand);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// Obtiene una propiedad por ID. Use expand=owner para incluir propietario.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] string? expand = null)
    {
        var expandFields = ParseExpandParameter(expand);
        var result = await _service.GetByIdAsync(id, expandFields);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { Message = result.Error });
    }

    private static string[] ParseExpandParameter(string? expand)
    {
        if (string.IsNullOrWhiteSpace(expand))
            return [];

        return [.. expand.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())];
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropiedadDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePropiedadDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
