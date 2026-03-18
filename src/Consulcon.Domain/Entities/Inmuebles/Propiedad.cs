using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Inmuebles;

public partial class Propiedad
{
    public int IdPropiedad { get; set; }

    public int IdManzano { get; set; }

    public string CodigoUnidad { get; set; } = null!;

    public string? NombreFuncional { get; set; }

    public decimal? SuperficieM2 { get; set; }

    /// <summary>
    /// Para prorrateo
    /// </summary>
    public decimal? PorcentajeParticipacion { get; set; }

    public decimal? ExpensaBaseDefecto { get; set; }

    /// <summary>
    /// Casa, Depto, Lote
    /// </summary>
    public string? Tipo { get; set; }

    public bool? Activo { get; set; }

    /// <summary>
    /// Saldo deudor actual de la unidad (Cobranzas)
    /// </summary>
    public decimal SaldoDeudor { get; set; }

    /// <summary>
    /// Saldo a favor actual de la unidad (Pagos anticipados / excedentes)
    /// </summary>
    public decimal SaldoAFavor { get; set; }

    public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

    public virtual Manzano IdManzanoNavigation { get; set; } = null!;
}
