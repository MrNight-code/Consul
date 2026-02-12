namespace Consulcon.Application.DTOs.Contabilidad.FiscalPeriods;

public class FiscalPeriodDto
{
    public int CondominioId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int? ClosedByUserId { get; set; }
}
