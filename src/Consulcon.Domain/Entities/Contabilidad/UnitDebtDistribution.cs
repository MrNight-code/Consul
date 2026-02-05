using System;

namespace Consulcon.Domain.Entities.Contabilidad
{
    public class UnitDebtDistribution
    {
        public int IdEgreso { get; set; }
        public int IdPropiedad { get; set; }
        
        // Datos de cálculo
        public decimal PorcentajeAplicado { get; set; }
        public decimal MontoCalculado { get; set; }
        
        // Auditoría
        public DateTime FechaCalculo { get; set; } = DateTime.UtcNow;

        public UnitDebtDistribution(int idEgreso, int idPropiedad, decimal porcentajeAplicado, decimal montoCalculado)
        {
            IdEgreso = idEgreso;
            IdPropiedad = idPropiedad;
            PorcentajeAplicado = porcentajeAplicado;
            MontoCalculado = montoCalculado;
        }
    }
}
