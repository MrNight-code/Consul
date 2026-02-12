using System.Collections.Concurrent;
using Consulcon.Application.DTOs.Contabilidad.FiscalPeriods;

namespace Consulcon.Infrastructure.Services.Contabilidad;

/// <summary>
/// Almacenamiento en memoria (Singleton) para períodos fiscales cerrados.
/// NOTA: Los datos se pierden al reiniciar el backend.
/// </summary>
public class InMemoryFiscalPeriodStore
{
    // Clave: (CondominioId, Year, Month) -> Información del período cerrado
    private readonly ConcurrentDictionary<(int CondominioId, int Year, int Month), ClosedPeriodInfo> _closedPeriods = new();

    // Log de auditoría en memoria
    private readonly List<FiscalPeriodAuditEntry> _auditLog = new();
    private readonly object _auditLock = new();

    /// <summary>
    /// Marca un período como cerrado.
    /// </summary>
    public bool ClosePeriod(int condominioId, int year, int month, int userId, string? traceId)
    {
        var key = (condominioId, year, month);
        var info = new ClosedPeriodInfo
        {
            ClosedAt = DateTime.UtcNow,
            ClosedByUserId = userId
        };

        if (_closedPeriods.TryAdd(key, info))
        {
            // Registrar en audit log
            lock (_auditLock)
            {
                _auditLog.Add(new FiscalPeriodAuditEntry
                {
                    CondominioId = condominioId,
                    Year = year,
                    Month = month,
                    Action = "CLOSED",
                    PerformedByUserId = userId,
                    PerformedAt = info.ClosedAt,
                    TraceId = traceId
                });
            }
            return true;
        }

        return false; // Ya estaba cerrado
    }

    /// <summary>
    /// Verifica si un período está cerrado.
    /// </summary>
    public bool IsClosed(int condominioId, int year, int month)
    {
        return _closedPeriods.ContainsKey((condominioId, year, month));
    }

    /// <summary>
    /// Verifica si una fecha cae en un período cerrado.
    /// </summary>
    public bool IsDateInClosedPeriod(int condominioId, DateTime date)
    {
        return IsClosed(condominioId, date.Year, date.Month);
    }

    /// <summary>
    /// Obtiene todos los períodos cerrados de un condominio.
    /// </summary>
    public List<FiscalPeriodDto> GetClosedPeriods(int condominioId)
    {
        return _closedPeriods
            .Where(kvp => kvp.Key.CondominioId == condominioId)
            .Select(kvp => new FiscalPeriodDto
            {
                CondominioId = kvp.Key.CondominioId,
                Year = kvp.Key.Year,
                Month = kvp.Key.Month,
                IsClosed = true,
                ClosedAt = kvp.Value.ClosedAt,
                ClosedByUserId = kvp.Value.ClosedByUserId
            })
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToList();
    }

    /// <summary>
    /// Obtiene el log de auditoría de un condominio.
    /// </summary>
    public List<FiscalPeriodAuditEntry> GetAuditLog(int condominioId)
    {
        lock (_auditLock)
        {
            return _auditLog
                .Where(e => e.CondominioId == condominioId)
                .OrderByDescending(e => e.PerformedAt)
                .ToList();
        }
    }

    /// <summary>
    /// Reabre un período previamente cerrado.
    /// </summary>
    public bool ReopenPeriod(int condominioId, int year, int month, int userId)
    {
        var key = (condominioId, year, month);
        
        if (_closedPeriods.TryRemove(key, out _))
        {
            lock (_auditLock)
            {
                _auditLog.Add(new FiscalPeriodAuditEntry
                {
                    CondominioId = condominioId,
                    Year = year,
                    Month = month,
                    Action = "REOPENED",
                    PerformedByUserId = userId,
                    PerformedAt = DateTime.UtcNow
                });
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// Obtiene información de un período específico si está cerrado.
    /// </summary>
    public ClosedPeriodInfo? GetClosedPeriodInfo(int condominioId, int year, int month)
    {
        _closedPeriods.TryGetValue((condominioId, year, month), out var info);
        return info;
    }
}

public class ClosedPeriodInfo
{
    public DateTime ClosedAt { get; set; }
    public int ClosedByUserId { get; set; }
}

public class FiscalPeriodAuditEntry
{
    public int CondominioId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Action { get; set; } = null!;
    public int PerformedByUserId { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? TraceId { get; set; }
}
