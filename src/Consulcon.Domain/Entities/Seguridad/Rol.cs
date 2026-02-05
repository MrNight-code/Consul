using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Seguridad;


public partial class Rol
{
    /// <summary>
    /// Antes: tipousuario
    /// </summary>
    public int IdRol { get; set; }

    /// <summary>
    /// Admin, Guardia, Vecino
    /// </summary>
    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<Permiso> IdPermisos { get; set; } = new List<Permiso>();
}
