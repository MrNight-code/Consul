using Consulcon.Application.DTOs.Contabilidad.FiscalPeriods;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;

namespace Consulcon.Infrastructure.Services.Contabilidad;

/// <summary>
/// Servicio para gestionar el cierre de períodos fiscales.
/// Los períodos cerrados se almacenan en memoria y se pierden al reiniciar.
/// </summary>
public class FiscalPeriodService : IFiscalPeriodService
{
    private readonly InMemoryFiscalPeriodStore _store;

    public FiscalPeriodService(InMemoryFiscalPeriodStore store)
    {
        _store = store;
    }

    public Result<FiscalPeriodDto> ClosePeriod(ClosePeriodRequest request, int userId, int condominioId)
    {
        // Validaciones
        if (condominioId <= 0)
            return Result.Fail<FiscalPeriodDto>("El ID de condominio es inválido.");

        if (request.Year < 2000 || request.Year > 2100)
            return Result.Fail<FiscalPeriodDto>("El año debe estar entre 2000 y 2100.");

        if (request.Month < 1 || request.Month > 12)
            return Result.Fail<FiscalPeriodDto>("El mes debe estar entre 1 y 12.");

        // Verificar que no sea un período futuro
        var now = DateTime.UtcNow;
        var periodEnd = new DateTime(request.Year, request.Month, 1).AddMonths(1).AddDays(-1);
        if (periodEnd > now)
            return Result.Fail<FiscalPeriodDto>("No se puede cerrar un período futuro o el mes actual antes de que termine.");

        // Verificar si ya está cerrado
        if (_store.IsClosed(condominioId, request.Year, request.Month))
            return Result.Fail<FiscalPeriodDto>($"El período {request.Month:D2}/{request.Year} ya está cerrado.");

        // Cerrar el período
        var traceId = Guid.NewGuid().ToString("N")[..8]; // Generar traceId simple
        var success = _store.ClosePeriod(condominioId, request.Year, request.Month, userId, traceId);

        if (!success)
            return Result.Fail<FiscalPeriodDto>("Error al cerrar el período. Por favor intente nuevamente.");

        var info = _store.GetClosedPeriodInfo(condominioId, request.Year, request.Month);

        return Result.Ok(new FiscalPeriodDto
        {
            CondominioId = condominioId,
            Year = request.Year,
            Month = request.Month,
            IsClosed = true,
            ClosedAt = info?.ClosedAt,
            ClosedByUserId = info?.ClosedByUserId
        });
    }

    public Result<bool> ReopenPeriod(ReopenPeriodRequest request, int userId, int condominioId)
    {
         if (condominioId <= 0)
            return Result.Fail<bool>("El ID de condominio es inválido.");

        if (!_store.IsClosed(condominioId, request.Year, request.Month))
            return Result.Fail<bool>($"El período {request.Month:D2}/{request.Year} no está cerrado.");

         var success = _store.ReopenPeriod(condominioId, request.Year, request.Month, userId);
         
         if (success)
            return Result.Ok(true);
         
         return Result.Fail<bool>("No se pudo reabrir el período.");
    }

    public bool IsPeriodClosed(int condominioId, DateTime date)
    {
        return _store.IsDateInClosedPeriod(condominioId, date);
    }

    public List<FiscalPeriodDto> GetClosedPeriods(int condominioId)
    {
        return _store.GetClosedPeriods(condominioId);
    }

    public List<FiscalPeriodAuditDto> GetAuditLog(int condominioId)
    {
        return _store.GetAuditLog(condominioId)
            .Select(e => new FiscalPeriodAuditDto
            {
                CondominioId = e.CondominioId,
                Year = e.Year,
                Month = e.Month,
                Action = e.Action,
                PerformedByUserId = e.PerformedByUserId,
                PerformedAt = e.PerformedAt,
                TraceId = e.TraceId
            })
            .ToList();
    }
}
