using System.Security.Claims;
using Consulcon.Application.DTOs.Contabilidad.FiscalPeriods;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

/// <summary>
/// Controller para gestión de períodos fiscales.
/// Permite cerrar períodos y consultar su estado.
/// </summary>
[ApiController]
[Route("api/fiscal-periods")]
public class FiscalPeriodsController : ControllerBase
{
    private readonly IFiscalPeriodService _fiscalPeriodService;

    public FiscalPeriodsController(IFiscalPeriodService fiscalPeriodService)
    {
        _fiscalPeriodService = fiscalPeriodService;
    }

    /// <summary>
    /// Cierra un período fiscal para un condominio.
    /// Infiere el ID de condominio desde el header X-Condominio-Id.
    /// </summary>
    [HttpPost("close")]
    public IActionResult ClosePeriod([FromBody] ClosePeriodRequest request)
    {
        if (!Request.Headers.TryGetValue("X-Condominio-Id", out var condoIdHeader) || !int.TryParse(condoIdHeader, out int condominioId))
        {
            return BadRequest("X-Condominio-Id header is missing or invalid.");
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = _fiscalPeriodService.ClosePeriod(request, userId, condominioId);

        if (result.IsSuccess)
        {
            return Ok(new
            {
                message = $"Período {request.Month:D2}/{request.Year} cerrado exitosamente.",
                period = result.Value
            });
        }

        return BadRequest(new
        {
            isSuccess = false,
            error = result.Error
        });
    }

    /// <summary>
    /// Reabre un período fiscal para un condominio.
    /// </summary>
    [HttpPost("reopen")]
    public IActionResult ReopenPeriod([FromBody] ReopenPeriodRequest request)
    {
        if (!Request.Headers.TryGetValue("X-Condominio-Id", out var condoIdHeader) || !int.TryParse(condoIdHeader, out int condominioId))
        {
            return BadRequest("X-Condominio-Id header is missing or invalid.");
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = _fiscalPeriodService.ReopenPeriod(request, userId, condominioId);

        if (result.IsSuccess)
        {
            return Ok(new
            {
                message = $"Período {request.Month:D2}/{request.Year} reabierto exitosamente."
            });
        }

        return BadRequest(new
        {
            isSuccess = false,
            error = result.Error
        });
    }

    /// <summary>
    /// Obtiene la lista de períodos cerrados para un condominio.
    /// </summary>
    [HttpGet]
    public IActionResult GetClosedPeriods()
    {
        if (!Request.Headers.TryGetValue("X-Condominio-Id", out var condoIdHeader) || !int.TryParse(condoIdHeader, out int condominioId))
        {
            return BadRequest("X-Condominio-Id header is missing or invalid.");
        }

        var periods = _fiscalPeriodService.GetClosedPeriods(condominioId);
        return Ok(new
        {
            condominioId,
            closedPeriods = periods,
            count = periods.Count
        });
    }

    /// <summary>
    /// Verifica si un período específico está cerrado.
    /// </summary>
    [HttpGet("status/{year}/{month}")]
    public IActionResult GetPeriodStatus(int year, int month)
    {
        if (!Request.Headers.TryGetValue("X-Condominio-Id", out var condoIdHeader) || !int.TryParse(condoIdHeader, out int condominioId))
        {
            return BadRequest("X-Condominio-Id header is missing or invalid.");
        }

        if (month < 1 || month > 12)
            return BadRequest("El mes debe estar entre 1 y 12.");

        var date = new DateTime(year, month, 1);
        var isClosed = _fiscalPeriodService.IsPeriodClosed(condominioId, date);

        return Ok(new
        {
            condominioId,
            year,
            month,
            isClosed,
            status = isClosed ? "CERRADO" : "ABIERTO"
        });
    }

    /// <summary>
    /// Obtiene el log de auditoría de cierres de período para un condominio.
    /// </summary>
    [HttpGet("audit-log")]
    public IActionResult GetAuditLog()
    {
        if (!Request.Headers.TryGetValue("X-Condominio-Id", out var condoIdHeader) || !int.TryParse(condoIdHeader, out int condominioId))
        {
            return BadRequest("X-Condominio-Id header is missing or invalid.");
        }

        var auditLog = _fiscalPeriodService.GetAuditLog(condominioId);
        return Ok(new
        {
            condominioId,
            auditLog,
            count = auditLog.Count
        });
    }
}
