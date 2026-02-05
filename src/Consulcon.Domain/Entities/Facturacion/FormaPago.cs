using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Facturacion;

public partial class FormaPago
{
    public int IdFormaPago { get; set; }

    /// <summary>
    /// Efectivo, Cheque, Transferencia
    /// </summary>
    public string Descripcion { get; set; } = null!;

    public int? IdCuentaContableAsociada { get; set; }

    public virtual ICollection<Egreso> Egresos { get; set; } = new List<Egreso>();

    public virtual PlanCuenta? IdCuentaContableAsociadaNavigation { get; set; }

    public virtual ICollection<TransaccionPago> TransaccionPagos { get; set; } = new List<TransaccionPago>();
}
