using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.General;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string RazonSocial { get; set; } = null!;

    public string? Nit { get; set; }

    public string? Contacto { get; set; }

    public string? Direccion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Egreso> Egresos { get; set; } = new List<Egreso>();
}
