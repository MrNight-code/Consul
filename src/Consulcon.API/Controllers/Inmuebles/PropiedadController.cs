using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Facturacion;
using Consulcon.Application.Interfaces.Inmuebles;
using Consulcon.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Inmuebles;

public class PropiedadController(IPropiedadService service, IDeudaService deudaService) : BaseController
{
    private readonly IDeudaService _deudaService = deudaService;
    private static readonly string[] OwnerExpand = ["owner"];

    [HttpGet("{id}/estado-cuenta")]
    public async Task<IActionResult> GetEstadoCuenta(int id)
    {
        var result = await _deudaService.GetEstadoCuentaByPropiedadAsync(id);
        if (!result.IsSuccess) return NotFound(new { Message = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene todas las propiedades. Use expand=owner para incluir propietarios.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? expand = null) 
        => HandleResult(await service.GetAllAsync(ParseExpandParameter(expand)));

    /// Obtiene propiedades por condominio. Use expand=owner para incluir propietarios.
    [HttpGet("condominio")]
    public async Task<IActionResult> GetByCondominio([FromQuery] string? expand = null) 
        => HandleResult(await service.GetByCondominioAsync(CondominioId, ParseExpandParameter(expand)));

    [HttpGet("condominio/with-owners")]
    public async Task<IActionResult> GetByCondominioWithOwners() 
        => HandleResult(await service.GetByCondominioAsync(CondominioId, OwnerExpand));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] string? expand = null) 
        => HandleResult(await service.GetByIdAsync(id, ParseExpandParameter(expand)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropiedadDto dto) 
        => HandleResult(await service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreatePropiedadDto dto) 
        => HandleResult(await service.UpdateAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) 
        => HandleResult(await service.DeleteAsync(id));

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParams parameters) 
        => HandleResult(await service.GetPagedAsync(CondominioId, parameters));

    private static string[] ParseExpandParameter(string? expand)
    {
        if (string.IsNullOrWhiteSpace(expand)) return [];
        return [.. expand.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())];
    }
}