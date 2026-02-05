using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contabilidad;

public partial class PlanCuenta
{
    public int IdCuenta { get; set; }

    /// <summary>
    /// Ej: 1.1.01
    /// </summary>
    public string CodigoCuenta { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Recursiva
    /// </summary>
    public int? IdCuentaPadre { get; set; }

    public int? NivelJerarquia { get; set; }

    /// <summary>
    /// Si/No
    /// </summary>
    public bool? EsImputable { get; set; }

    public virtual ICollection<AsientoDetalle> AsientoDetalles { get; set; } = new List<AsientoDetalle>();

    public virtual ICollection<Banco> Bancos { get; set; } = new List<Banco>();

    public virtual ICollection<FormaPago> FormaPagos { get; set; } = new List<FormaPago>();

    public virtual PlanCuenta? IdCuentaPadreNavigation { get; set; }

    public virtual ICollection<PlanCuenta> InverseIdCuentaPadreNavigation { get; set; } = new List<PlanCuenta>();

    public virtual ICollection<CatalogoServicio> IdServicios { get; set; } = new List<CatalogoServicio>();
}
