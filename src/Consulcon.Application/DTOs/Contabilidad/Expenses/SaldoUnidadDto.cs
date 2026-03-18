namespace Consulcon.Application.DTOs.Contabilidad.Expenses
{
    public class SaldoUnidadDto
    {
        public int FkPropiedad { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public string? Propietario { get; set; }
        public decimal SaldoActual { get; set; }
        public bool TienePropietario { get; set; }
    }
}
