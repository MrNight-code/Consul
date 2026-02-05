using System;
using System.Collections.Generic;
using Consulcon.Domain.Entities;

namespace Consulcon.Domain.Entities.Financiero;

public partial class ChargeConcept
{
    public int Id { get; set; }

    public int CondominiumId { get; set; }

    /// <summary>
    /// Name of the concept (e.g. "Expensas Ordinarias", "Multa por Ruidos Molestos")
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Codes/Initializers for accounting or internal use
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// If true, this charge is expected to be applied periodically
    /// </summary>
    public bool IsRecurrent { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual Condominio Condominium { get; set; } = null!;
}
