using Consulcon.Application.DTOs;
using Consulcon.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers;

public class CobranzasController(ICobranzaService cobranzaService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> RegistrarCobranza([FromBody] CobranzaRequest request) 
        => HandleResult(await cobranzaService.RegistrarCobranzaAsync(request));

    [HttpGet("{unitId}")]
    public async Task<IActionResult> ObtenerHistorial(int unitId) 
        => HandleResult(await cobranzaService.ObtenerHistorialAsync(unitId));
}