using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;

namespace Consulcon.API.Controllers.Facturacion;

[ApiController]
[Route("api/[controller]")]
public class PagoController(IPagoService service) : ControllerBase
{
    private readonly IPagoService _service = service;

    [HttpGet("deuda/{deudaId}")]
    public async Task<IActionResult> GetByDeuda(int deudaId)
    {
        var result = await _service.GetByDeudaAsync(deudaId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarPago([FromBody] CreatePagoDto dto)
    {
        var result = await _service.RegistrarPagoAsync(dto);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id}/generar-recibo")]
    public async Task<IActionResult> GenerarRecibo(int id, [FromServices] IReceiptGenerationService receiptService)
    {
        try 
        {
            var pago = await receiptService.GenerateReceiptAsync(id);
            // We return the updated entity (or a mapped DTO if we had one ready)
            // returning key info manually for now
            return Ok(new 
            {
                Id = pago.IdPago,
                RutaPdf = pago.ReciboUrl, // Adapted property
                FechaGeneracion = pago.FechaRecibo, // Adapted property
                Mensaje = "Recibo generado inmutablemente con fecha del servidor."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("recibos")]
    public async Task<IActionResult> GetGeneratedReceipts([FromQuery] ReceiptFilterDto filter, [FromServices] IReceiptGenerationService receiptService)
    {
        var receipts = await receiptService.GetGeneratedReceiptsAsync(filter);
        return Ok(receipts);
    }

    [HttpGet("recibos/{filename}")]
    public IActionResult GetRecibo(string filename)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedReceipts");
        var filePath = Path.Combine(folderPath, filename);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("El recibo no existe.");
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/pdf", filename);
    }
}
