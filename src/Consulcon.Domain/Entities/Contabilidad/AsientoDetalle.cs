using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contabilidad;

public partial class AsientoDetalle
{
    public int IdAsientoDet { get; set; }

    public int IdAsiento { get; set; }

    public int IdCuenta { get; set; }

    public string? GlosaLinea { get; set; }

    public decimal? Debe { get; set; }

    public decimal? Haber { get; set; }

    public virtual AsientoContable IdAsientoNavigation { get; set; } = null!;

    public virtual PlanCuenta IdCuentaNavigation { get; set; } = null!;
}
