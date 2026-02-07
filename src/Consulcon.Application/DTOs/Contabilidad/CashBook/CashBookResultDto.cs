namespace Consulcon.Application.DTOs.Contabilidad.CashBook;

/// <summary>
/// Paginated result of the cash book report.
/// </summary>
public class CashBookResultDto
{
    /// <summary>
    /// Balance before the StartDate (sum of all prior transactions).
    /// </summary>
    public decimal InitialBalance { get; set; }
    
    /// <summary>
    /// Balance at the end of the current page.
    /// </summary>
    public decimal FinalBalance { get; set; }
    
    public List<CashBookEntryDto> Entries { get; set; } = [];
    
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
}
