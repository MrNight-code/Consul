namespace Consulcon.Application.DTOs.Financiero;

public class FinancialConfigDto
{
    public int Id { get; set; }
    public int CondominiumId { get; set; }
    public decimal MonthlyInterestRate { get; set; }
    public int GraceDays { get; set; }
}

public class UpdateFinancialConfigDto
{
    public decimal MonthlyInterestRate { get; set; }
    public int GraceDays { get; set; }
}
