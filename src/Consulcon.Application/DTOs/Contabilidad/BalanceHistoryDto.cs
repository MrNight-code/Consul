namespace Consulcon.Application.DTOs.Contabilidad;

public class BalanceHistoryDto
{
    public DateTime Date { get; set; }
    public decimal Balance { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    // Transaction Details
    public int? TransactionId { get; set; }
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public string? Beneficiary { get; set; }
    public string? TransactionType { get; set; } // "Egreso", "Ingreso", "Ajuste"
}