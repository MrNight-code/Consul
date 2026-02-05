using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Inmuebles;

public partial class Condominio
{
    public int IdCondominio { get; set; }

    public string? Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public decimal? SuperficieTotalM2 { get; set; }

    public int IdAdminPersona { get; set; }

    public string? ConfigDiaCobro { get; set; }

    public string? Logo { get; set; }

    public virtual ICollection<AsientoContable> AsientoContables { get; set; } = [];

    public virtual ICollection<ComunicadoBlog> ComunicadoBlogs { get; set; } = [];

    public virtual ICollection<ConfigAvisoCobranza> ConfigAvisoCobranzas { get; set; } = [];

    public virtual ICollection<Egreso> Egresos { get; set; } = [];

    public virtual Persona IdAdminPersonaNavigation { get; set; } = null!;

    public virtual ICollection<Manzano> Manzanos { get; set; } = [];

    public virtual ICollection<RecursoComun> RecursoComuns { get; set; } = [];
}
