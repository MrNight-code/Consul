namespace Consulcon.Application.DTOs.Contabilidad.CashBook;

/// <summary>
/// Represents a single entry (movement) in the cash book.
/// </summary>
public class CashBookEntryDto
{
    public int Id { get; set; }
    
    /// <summary>
    /// "IN" for income (cobranzas), "OUT" for expenses (egresos).
    /// </summary>
    public string Type { get; set; } = null!;
    
    public DateTime Date { get; set; }
    public string Description { get; set; } = null!;
    public string? Reference { get; set; }
    
    /// <summary>
    /// Positive for income, Negative for expenses.
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Running balance after this entry.
    /// </summary>
    public decimal Balance { get; set; }
    
    public string AccountName { get; set; } = null!;
    public int AccountId { get; set; }
    
    /// <summary>
    /// If true, this entry is voided and doesn't affect balance calculation.
    /// </summary>
    public bool IsVoided { get; set; }
}
