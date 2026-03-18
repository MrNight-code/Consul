using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Facturacion;

public class PagoController(
    IPagoService service, 
    IReceiptGenerationService receiptService) : BaseController
{
    [HttpGet("deuda/{deudaId}")]
    public async Task<IActionResult> GetByDeuda(int deudaId) 
        => HandleResult(await service.GetByDeudaAsync(deudaId));

    [HttpPost]
    public async Task<IActionResult> RegistrarPago([FromBody] CreatePagoDto dto) 
        => HandleResult(await service.RegistrarPagoAsync(dto));

    [HttpPost("{id}/generar-recibo")]
    public async Task<IActionResult> GenerarRecibo(int id)
    {
        try 
        {
            var pago = await receiptService.GenerateReceiptAsync(id);
            
            return Ok(new 
            {
                Id = pago.IdPago,
                RutaPdf = pago.ReciboUrl, 
                FechaGeneracion = pago.FechaRecibo, 
                Mensaje = "Recibo generado inmutablemente con fecha del servidor."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("recibos")]
    public async Task<IActionResult> GetGeneratedReceipts(
        [FromQuery] Consulcon.Domain.Common.PaginationParams parameters, 
        [FromQuery] string? medio, 
        [FromQuery] int? propiedadId) 
        => Ok(await receiptService.GetGeneratedReceiptsAsync(parameters, medio, propiedadId));

    [HttpGet("recibos/{filename}")]
    public IActionResult GetRecibo(string filename, [FromServices] Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        var outputFolder = configuration["ReceiptSettings:OutputFolder"] ?? "GeneratedReceipts";
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), outputFolder);
        var filePath = Path.Combine(folderPath, filename);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new { message = "El recibo no existe." }); 

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/pdf", filename);
    }

    [HttpGet("recibos/batch")]
    public async Task<IActionResult> GetBatchRecibos([FromQuery] int mes, [FromQuery] int anio)
    {
        var stream = await receiptService.GetBatchZipAsync(mes, anio);
        return File(stream, "application/zip", $"Recibos_{mes}_{anio}.zip");
    }
}
