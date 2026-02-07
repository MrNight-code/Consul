using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;

namespace Consulcon.API.Controllers.Contabilidad;

[ApiController]
[Route("api/[controller]")]
public class ContabilidadController(IContabilidadService service) : ControllerBase
{
    [HttpGet("plancuentas")]
    public async Task<IActionResult> GetPlanCuentas()
    {
        var result = await service.GetPlanCuentasAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("plancuentas")]
    public async Task<IActionResult> CreateCuenta([FromBody] PlanCuentaDto dto)
    {
        var result = await service.CreateCuentaAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("asientos/condominio/{condominioId}")]
    public async Task<IActionResult> GetAsientos(int condominioId)
    {
        var result = await service.GetAsientosByCondominioAsync(condominioId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("asientos")]
    public async Task<IActionResult> RegistrarAsiento([FromBody] CreateAsientoDto dto)
    {
        var result = await service.RegistrarAsientoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("autorizaciones")]
    public async Task<IActionResult> GetAutorizaciones()
    {
        var result = await service.GetAutorizacionesAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("autorizaciones")]
    public async Task<IActionResult> CreateAutorizacion([FromBody] AutorizacionGastoDto dto)
    {
        var result = await service.CreateAutorizacionAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("/api/expenses/{id}/void")]
    public async Task<IActionResult> VoidExpense(int id, [FromBody] VoidExpenseRequest request)
    {
        var result = await service.VoidExpenseAsync(id, request);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(new { message = "Gasto anulado exitosamente y saldo revertido." });
    }
}


