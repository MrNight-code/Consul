using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Reservas;

public partial class Reserva
{
    /// <summary>
    /// Antes: evento
    /// </summary>
    public int IdReserva { get; set; }

    public int IdRecurso { get; set; }

    /// <summary>
    /// Quien reserva
    /// </summary>
    public int IdContrato { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public int? CantidadInvitados { get; set; }

    public string? Motivo { get; set; }

    public string? AmenizadoPor { get; set; }

    public decimal? MontoTotalCobrado { get; set; }

    /// <summary>
    /// PENDIENTE, CONFIRMADA, FINALIZADA
    /// </summary>
    public string? Estado { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual RecursoComun IdRecursoNavigation { get; set; } = null!;
}
