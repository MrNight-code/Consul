using System;

namespace Consulcon.Domain.Entities.Contabilidad;

public partial class EgresoDetalle
{
    public int IdEgresoDetalle { get; set; }

    public int IdEgreso { get; set; }

    public string Concepto { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Egreso IdEgresoNavigation { get; set; } = null!;
}
