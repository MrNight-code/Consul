using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contratos;

public partial class CatalogoServicio
{
    /// <summary>
    /// Antes: serviciopago
    /// </summary>
    public int IdServicio { get; set; }

    /// <summary>
    /// Agua, Luz, Multa, Expensa
    /// </summary>
    public string Nombre { get; set; } = null!;

    public decimal? CostoBase { get; set; }

    public bool? EsRecurrente { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<ContratoServicioSuscrito> ContratoServicioSuscritos { get; set; } = new List<ContratoServicioSuscrito>();

    public virtual ICollection<DeudaDetalle> DeudaDetalles { get; set; } = new List<DeudaDetalle>();

    public virtual ICollection<PlanCuenta> IdCuentaIngresos { get; set; } = new List<PlanCuenta>();
}
