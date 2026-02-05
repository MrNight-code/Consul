using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contratos;

public partial class ContratoParticipante
{
    public int IdContrato { get; set; }

    public int IdPersona { get; set; }

    /// <summary>
    /// Titular, Inquilino, Garante
    /// </summary>
    public string RolContrato { get; set; } = null!;

    public DateOnly? FechaAlta { get; set; }

    public DateOnly? FechaBaja { get; set; }

    public bool? Activo { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual Persona IdPersonaNavigation { get; set; } = null!;
}
