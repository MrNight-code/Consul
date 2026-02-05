using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Seguridad;


public partial class Permiso
{
    public int IdPermiso { get; set; }

    /// <summary>
    /// Antes: permiso
    /// </summary>
    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Rol> IdRols { get; set; } = new List<Rol>();
}
