using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Reservas;

public partial class RecursoComun
{
    public int IdRecurso { get; set; }

    public int IdCondominio { get; set; }

    /// <summary>
    /// Churrasquera, Salon
    /// </summary>
    public string Nombre { get; set; } = null!;

    public decimal? CostoReserva { get; set; }

    public decimal? CostoGarantia { get; set; }

    /// <summary>
    /// Antes en tabla evento
    /// </summary>
    public string? ColorCalendario { get; set; }

    public virtual Condominio IdCondominioNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
