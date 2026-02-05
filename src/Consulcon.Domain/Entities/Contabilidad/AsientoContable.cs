using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contabilidad;

public partial class AsientoContable
{
    public int IdAsiento { get; set; }

    public int IdCondominio { get; set; }

    public DateTime FechaContable { get; set; }

    public string? GlosaGeneral { get; set; }

    /// <summary>
    /// Diario, Ajuste, Cierre
    /// </summary>
    public string? TipoAsiento { get; set; }

    public string? NroDocumentoRespaldo { get; set; }

    /// <summary>
    /// Link a Tesoreria
    /// </summary>
    public int? IdTransaccionOrigenPago { get; set; }

    /// <summary>
    /// Link a Tesoreria
    /// </summary>
    public int? IdTransaccionOrigenEgreso { get; set; }

    public virtual ICollection<AsientoDetalle> AsientoDetalles { get; set; } = new List<AsientoDetalle>();

    public virtual Condominio IdCondominioNavigation { get; set; } = null!;

    public virtual Egreso? IdTransaccionOrigenEgresoNavigation { get; set; }

    public virtual TransaccionPago? IdTransaccionOrigenPagoNavigation { get; set; }
}
