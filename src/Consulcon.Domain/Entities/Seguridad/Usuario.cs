using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Seguridad;


public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdPersona { get; set; }

    public string Username { get; set; } = null!;

    /// <summary>
    /// Antes: contrasena
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public bool? EstaHabilitado { get; set; }

    public int? IdRolPrincipal { get; set; }

    public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

    public virtual ICollection<DeudaCabecera> DeudaCabeceras { get; set; } = new List<DeudaCabecera>();

    public virtual ICollection<Egreso> Egresos { get; set; } = new List<Egreso>();

    public virtual Persona IdPersonaNavigation { get; set; } = null!;

    public virtual Rol? IdRolPrincipalNavigation { get; set; }
}
