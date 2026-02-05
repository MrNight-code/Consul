using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.General;

public partial class Banco
{
    public int IdBanco { get; set; }

    public string NombreEntidad { get; set; } = null!;

    public string? NumeroCuenta { get; set; }

    public string? Moneda { get; set; }

    public string Tipo { get; set; } = "BANCO";

    public bool? Activo { get; set; }

    public int? IdCuentaContableAsociada { get; set; }

    public virtual ICollection<Egreso> Egresos { get; set; } = [];

    public virtual PlanCuenta? IdCuentaContableAsociadaNavigation { get; set; }

    public virtual ICollection<TransaccionPago> TransaccionPagos { get; set; } = [];
}
