using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;

namespace Consulcon.API.Controllers.Contabilidad;

[ApiController]
[Route("api/[controller]")]
public class TesoreriaController(ITesoreriaService service, Consulcon.Application.Interfaces.Facturacion.IExpenseReceiptGenerationService receiptService) : ControllerBase
{


    [HttpGet("formaspago")]
    public async Task<IActionResult> GetFormasPago()
    {
        var result = await service.GetFormasPagoAsync();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("formaspago")]
    public async Task<IActionResult> CreateFormaPago([FromBody] FormaPagoDto dto)
    {
        var result = await service.CreateFormaPagoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("egresos/condominio/{condominioId}")]
    public async Task<IActionResult> GetEgresos(int condominioId)
    {
        var result = await service.GetEgresosByCondominioAsync(condominioId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("egresos")]
    public async Task<IActionResult> RegistrarEgreso([FromBody] CreateEgresoDto dto)
    {
        var result = await service.RegistrarEgresoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("egresos/{id}/comprobante")]
    public async Task<IActionResult> GetComprobante(int id)
    {
        try
        {
            var stream = await receiptService.GenerateReceiptAsync(id);
            var fileName = $"Egreso_{id}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            return File(stream, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
