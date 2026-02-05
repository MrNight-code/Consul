using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contratos;

public partial class LecturaServicio
{
    public int IdLectura { get; set; }

    public int IdSuscripcion { get; set; }

    public int Anio { get; set; }

    public int Mes { get; set; }

    /// <summary>
    /// Para agua/luz variable
    /// </summary>
    public decimal? ValorLeido { get; set; }

    public decimal MontoCalculado { get; set; }

    public DateOnly? FechaLectura { get; set; }

    public virtual ContratoServicioSuscrito IdSuscripcionNavigation { get; set; } = null!;
}
