namespace Consulcon.Application.DTOs.Contabilidad.CashBook;

/// <summary>
/// Query parameters for Cash Book report generation.
/// </summary>
public class CashBookQuery
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Optional: Filter by specific financial account (Banco).
    /// </summary>
    public int? FinancialAccountId { get; set; }
    
    /// <summary>
    /// If true, includes voided transactions visually (shown but excluded from balance calculation).
    /// </summary>
    public bool IncludeVoided { get; set; } = false;
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
