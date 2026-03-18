

using System.Collections.Generic;

namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class ConciliarExpensasRequestDto
    {
        public decimal MontoTotal { get; set; }
        public List<ExpensaDistribucionDto> Distribucion { get; set; } = new();
        public int FkCondominio { get; set; }
    }
}
