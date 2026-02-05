using System;

namespace Consulcon.Application.DTOs
{
    public class CobranzaDto
    {
        public int IdPago { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string? MetodoPago { get; set; }
        public string? Referencia { get; set; }
        public string? Estado { get; set; }
        public string? Observaciones { get; set; }
        public string ConceptoDeuda { get; set; } = string.Empty;
    }
}
