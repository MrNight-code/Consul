using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contratos;

public partial class Contrato
{
    public int IdContrato { get; set; }

    public int IdPropiedad { get; set; }

    public DateOnly? FechaFirma { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public DateOnly? FechaIngresoReal { get; set; }

    public decimal MontoExpensaPactada { get; set; }

    /// <summary>
    /// Vigente, Finalizado, Rescindido
    /// </summary>
    public string? Estado { get; set; }

    public string? MotivoBaja { get; set; }

    public int? IdUsuarioCreador { get; set; }

    public virtual ICollection<ContratoParticipante> ContratoParticipantes { get; set; } = new List<ContratoParticipante>();

    public virtual ICollection<ContratoServicioSuscrito> ContratoServicioSuscritos { get; set; } = new List<ContratoServicioSuscrito>();

    public virtual ICollection<DeudaCabecera> DeudaCabeceras { get; set; } = new List<DeudaCabecera>();

    public virtual Propiedad IdPropiedadNavigation { get; set; } = null!;

    public virtual Usuario? IdUsuarioCreadorNavigation { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
