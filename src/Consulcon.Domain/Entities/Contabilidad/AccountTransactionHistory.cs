using System;

namespace Consulcon.Domain.Entities.Contabilidad
{
    public class AccountTransactionHistory
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public int? ExpenseId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;

        // Navigation properties
        public virtual Consulcon.Domain.Entities.General.Banco Account { get; set; } = null!;
        public virtual Egreso? Expense { get; set; }
    }
}
