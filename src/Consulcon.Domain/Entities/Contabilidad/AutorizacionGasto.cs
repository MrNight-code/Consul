using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contabilidad;

public partial class AutorizacionGasto
{
    public int IdAutorizacion { get; set; }

    /// <summary>
    /// Niveles de firma para gastos
    /// </summary>
    public string Descripcion { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual ICollection<Egreso> Egresos { get; set; } = new List<Egreso>();
}
