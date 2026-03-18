using Consulcon.Application.DTOs.Contabilidad.FiscalPeriods;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

public class FiscalPeriodsController(IFiscalPeriodService service) : BaseController
{
    [HttpPost("close")]
    public IActionResult ClosePeriod([FromBody] ClosePeriodRequest request) 
        => HandleResult(service.ClosePeriod(request, UserId, CondominioId));

    [HttpPost("reopen")]
    public IActionResult ReopenPeriod([FromBody] ReopenPeriodRequest request) 
        => HandleResult(service.ReopenPeriod(request, UserId, CondominioId));

    [HttpGet]
    public IActionResult GetClosedPeriods() 
    {
        var periods = service.GetClosedPeriods(CondominioId);
        return Ok(new
        {
            condominioId = CondominioId,
            closedPeriods = periods,
            count = periods.Count
        });
    }

    [HttpGet("status/{year}/{month}")]
    public IActionResult GetPeriodStatus(int year, int month)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { message = "El mes debe estar entre 1 y 12." });

        var date = new DateTime(year, month, 1);
        var isClosed = service.IsPeriodClosed(CondominioId, date);

        return Ok(new
        {
            condominioId = CondominioId,
            year,
            month,
            isClosed,
            status = isClosed ? "CERRADO" : "ABIERTO"
        });
    }

    [HttpGet("audit-log")]
    public IActionResult GetAuditLog()
    {
        var auditLog = service.GetAuditLog(CondominioId);
        return Ok(new
        {
            condominioId = CondominioId,
            auditLog,
            count = auditLog.Count
        });
    }
}