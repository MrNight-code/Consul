using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Facturacion;

public partial class DeudaDetalle
{
    public int IdDeudaDet { get; set; }

    public int IdDeuda { get; set; }

    /// <summary>
    /// Origen del cobro
    /// </summary>
    public int IdServicio { get; set; }

    /// <summary>
    /// Ej: Expensa Mayo 2025
    /// </summary>
    public string Concepto { get; set; } = null!;

    public decimal MontoUnitario { get; set; }

    public decimal? Cantidad { get; set; }

    public decimal Subtotal { get; set; }

    public virtual DeudaCabecera IdDeudaNavigation { get; set; } = null!;

    public virtual CatalogoServicio IdServicioNavigation { get; set; } = null!;
}
