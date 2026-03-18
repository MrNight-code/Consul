using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Consulcon.API.Controllers.Facturacion;

[ApiController]
[Authorize]
[Route("api/reports")]
public class ReceiptReportsController(
    IReceiptGenerationService receiptService,
    Consulcon.Domain.Interfaces.ICurrentTenantService tenantService,
    ILogger<ReceiptReportsController> logger) : ControllerBase
{
    private readonly IReceiptGenerationService _receiptService = receiptService;
    private readonly Consulcon.Domain.Interfaces.ICurrentTenantService _tenantService = tenantService;
    private readonly ILogger<ReceiptReportsController> _logger = logger;

    [HttpPost("receipts-batch")]
    public async Task<IActionResult> GenerateBatchReceipts([FromBody] BatchReceiptRequestDto request)
    {
        if (!_tenantService.CondominioId.HasValue)
        {
            _logger.LogWarning("Batch receipts request without X-Condominio-Id header.");
            return BadRequest(new { Message = "El encabezado 'X-Condominio-Id' es requerido para generar reportes." });
        }

        _logger.LogInformation("Generating batch receipts for Condominio {CondominioId} from {StartDate} to {EndDate}", 
            _tenantService.CondominioId, request.StartDate, request.EndDate);

        try
        {
            var pdfBytes = await _receiptService.GenerateBatchReceiptsPdfAsync(request);
            var fileName = $"Recibos_Batch_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("No receipts found for the given criteria: {Message}", ex.Message);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating batch receipts");
            return BadRequest(new { Message = ex.Message });
        }
    }
}
