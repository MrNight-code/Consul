using System.Collections.Generic;

namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class ConciliacionExpensaDto
    {
        public decimal MontoBruto { get; set; }
        public decimal TotalSaldosAFavor { get; set; }
        public decimal MontoNeto { get; set; }
        public int UnidadesConExcepcion { get; set; }
        public int UnidadesSinPropietario { get; set; }
        public List<UnidadExcepcionDto> Excepciones { get; set; } = new();
    }
}
