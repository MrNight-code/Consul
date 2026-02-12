using Consulcon.Application.DTOs.Contabilidad.FiscalPeriods;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Interfaces.Contabilidad;

public interface IFiscalPeriodService
{
    /// <summary>
    /// Cierra un período fiscal para un condominio específico.
    /// Una vez cerrado, no se pueden agregar egresos a ese período.
    /// </summary>
    Result<FiscalPeriodDto> ClosePeriod(ClosePeriodRequest request, int userId, int condominioId);

    /// <summary>
    /// Reabre un período fiscal previamente cerrado.
    /// Solo permitido para roles administrativos.
    /// </summary>
    Result<bool> ReopenPeriod(ReopenPeriodRequest request, int userId, int condominioId);

    /// <summary>
    /// Verifica si una fecha específica cae en un período cerrado para un condominio.
    /// </summary>
    bool IsPeriodClosed(int condominioId, DateTime date);

    /// <summary>
    /// Obtiene la lista de períodos cerrados para un condominio.
    /// </summary>
    List<FiscalPeriodDto> GetClosedPeriods(int condominioId);

    /// <summary>
    /// Obtiene los registros de auditoría de cierres de período.
    /// </summary>
    List<FiscalPeriodAuditDto> GetAuditLog(int condominioId);
}

public class FiscalPeriodAuditDto
{
    public int CondominioId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Action { get; set; } = null!;
    public int PerformedByUserId { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? TraceId { get; set; }
}
