using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.General;

public partial class Persona
{
    public int IdPersona { get; set; }

    /// <summary>
    /// Antes: nombre
    /// </summary>
    public string NombreCompleto { get; set; } = null!;

    public string? Ci { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Sexo { get; set; }

    public string? EstadoCivil { get; set; }

    public bool? EsActivo { get; set; }

    /// <summary>
    /// Recursiva: Para hijos/dependientes
    /// </summary>
    public int? IdFamiliarResponsable { get; set; }

    public virtual ICollection<Condominio> Condominios { get; set; } = new List<Condominio>();

    public virtual ICollection<ContratoParticipante> ContratoParticipantes { get; set; } = new List<ContratoParticipante>();

    public virtual ICollection<Egreso> Egresos { get; set; } = new List<Egreso>();

    public virtual Persona? IdFamiliarResponsableNavigation { get; set; }

    public virtual ICollection<Persona> InverseIdFamiliarResponsableNavigation { get; set; } = new List<Persona>();

    public virtual ICollection<MedioContacto> MedioContactos { get; set; } = new List<MedioContacto>();

    public virtual ICollection<TransaccionPago> TransaccionPagos { get; set; } = new List<TransaccionPago>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
