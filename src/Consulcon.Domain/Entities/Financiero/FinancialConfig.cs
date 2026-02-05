using System;
using System.Collections.Generic;
using Consulcon.Domain.Entities;

namespace Consulcon.Domain.Entities.Financiero;

public partial class FinancialConfig
{
    public int Id { get; set; }

    public int CondominiumId { get; set; }

    /// <summary>
    /// Monthly Interest Rate for late payments (percentage, e.g. 2.50 for 2.5%)
    /// </summary>
    public decimal MonthlyInterestRate { get; set; }

    /// <summary>
    /// Days of grace before interest is applied
    /// </summary>
    public int GraceDays { get; set; }

    // Navigation property
    public virtual Condominio Condominium { get; set; } = null!;
}
