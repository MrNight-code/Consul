using Consulcon.Application.DTOs;
using Consulcon.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Facturacion;

[ApiController]
[Route("api/cobranzas")]
public class CobranzaController(ICobranzaService service) : ControllerBase
{
    private readonly ICobranzaService _service = service;

    [HttpPost]
    public async Task<IActionResult> RegistrarCobranza([FromBody] CobranzaRequest request)
    {
        var result = await _service.RegistrarCobranzaAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{unitId}")]
    public async Task<IActionResult> ObtenerHistorial(int unitId)
    {
        var result = await _service.ObtenerHistorialAsync(unitId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
