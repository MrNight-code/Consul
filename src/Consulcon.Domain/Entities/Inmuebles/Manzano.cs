using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Inmuebles;

public partial class Manzano
{
    public int IdManzano { get; set; }

    public int IdCondominio { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Nombre { get; set; }

    public virtual Condominio IdCondominioNavigation { get; set; } = null!;

    public virtual ICollection<Propiedad> Propiedads { get; set; } = new List<Propiedad>();
}
