using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contratos;

public partial class ContratoServicioSuscrito
{
    /// <summary>
    /// Antes: servicio_contrato
    /// </summary>
    public int IdSuscripcion { get; set; }

    public int IdContrato { get; set; }

    public int IdServicio { get; set; }

    /// <summary>
    /// Si difiere del base
    /// </summary>
    public decimal? CostoPersonalizado { get; set; }

    public bool? Activo { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual CatalogoServicio IdServicioNavigation { get; set; } = null!;

    public virtual ICollection<LecturaServicio> LecturaServicios { get; set; } = new List<LecturaServicio>();
}
