using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Application.Interfaces.Facturacion;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

public class TesoreriaController(
    ITesoreriaService service, 
    IExpenseReceiptGenerationService receiptService) : BaseController
{
    [HttpGet("formaspago")]
    public async Task<IActionResult> GetFormasPago() 
        => HandleResult(await service.GetFormasPagoAsync());

    [HttpPost("formaspago")]
    public async Task<IActionResult> CreateFormaPago([FromBody] FormaPagoDto dto) 
        => HandleResult(await service.CreateFormaPagoAsync(dto));

    [HttpGet("egresos")]
    public async Task<IActionResult> GetEgresos() 
        => HandleResult(await service.GetEgresosByCondominioAsync(CondominioId));

    [HttpPost("egresos")]
    public async Task<IActionResult> RegistrarEgreso([FromBody] CreateEgresoDto dto) 
        => HandleResult(await service.RegistrarEgresoAsync(dto));

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
            return BadRequest(new { message = ex.Message });
        }
    }
}