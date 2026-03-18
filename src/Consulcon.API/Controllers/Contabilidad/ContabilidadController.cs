using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

public class ContabilidadController(IContabilidadService service) : BaseController
{
    [HttpGet("plancuentas")]
    public async Task<IActionResult> GetPlanCuentas() 
        => HandleResult(await service.GetPlanCuentasAsync());

    [HttpPost("plancuentas")]
    public async Task<IActionResult> CreateCuenta([FromBody] PlanCuentaDto dto) 
        => HandleResult(await service.CreateCuentaAsync(dto));

    [HttpGet("asientos")]
    public async Task<IActionResult> GetAsientos() 
        => HandleResult(await service.GetAsientosByCondominioAsync(CondominioId));

    [HttpPost("asientos")]
    public async Task<IActionResult> RegistrarAsiento([FromBody] CreateAsientoDto dto) 
        => HandleResult(await service.RegistrarAsientoAsync(dto));

    [HttpGet("autorizaciones")]
    public async Task<IActionResult> GetAutorizaciones() 
        => HandleResult(await service.GetAutorizacionesAsync());

    [HttpPost("autorizaciones")]
    public async Task<IActionResult> CreateAutorizacion([FromBody] AutorizacionGastoDto dto) 
        => HandleResult(await service.CreateAutorizacionAsync(dto));

    [HttpPost("/api/expenses/{id}/void")]
    public async Task<IActionResult> VoidExpense(int id, [FromBody] VoidExpenseRequest request) 
        => HandleResult(await service.VoidExpenseAsync(id, request));
}