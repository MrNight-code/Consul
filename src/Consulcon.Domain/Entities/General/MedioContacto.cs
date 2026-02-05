using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.General;

public partial class MedioContacto
{
    public int IdContacto { get; set; }

    public int IdPersona { get; set; }

    /// <summary>
    /// Telefono, Celular, Email, Facebook
    /// </summary>
    public string Tipo { get; set; } = null!;

    /// <summary>
    /// El numero o correo
    /// </summary>
    public string Valor { get; set; } = null!;

    public bool? EsPrincipal { get; set; }

    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
