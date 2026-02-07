using System;

namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class RegisterExpenseCommand
    {
        public int CondominioId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ExpenseDate { get; set; }
        public string? InvoiceNumber { get; set; }
        public int? ProviderId { get; set; }
        public int CategoryId { get; set; } // IdAutorizacion / Concepto
        public int PaymentMethodId { get; set; } // IdFormaPago
    }
}
