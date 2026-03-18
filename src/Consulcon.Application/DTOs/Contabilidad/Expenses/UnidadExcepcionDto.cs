namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class UnidadExcepcionDto
    {
        public int FkPropiedad { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public string? Propietario { get; set; }
        public decimal SaldoAFavor { get; set; }
        public decimal MontoExpensa { get; set; }
        public decimal MontoNeto { get; set; }
        public bool TienePropietario { get; set; }
        public string TipoExcepcion { get; set; } = "Pago Adelantado";
    }
}
