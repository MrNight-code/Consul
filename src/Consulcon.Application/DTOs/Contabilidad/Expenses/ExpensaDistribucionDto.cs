namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class ExpensaDistribucionDto
    {
        public int Id { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public string Propietario { get; set; } = string.Empty;
        public decimal PorcentajeIncidencia { get; set; }
        public decimal MontoAPagar { get; set; }
        public int? FkPropiedad { get; set; }
        public int? FkContrato { get; set; }
        public System.DateTime? FechaCobro { get; set; }
    }
}
