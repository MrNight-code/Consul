using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Facturacion;

public partial class DeudaCabecera
{
    public int IdDeuda { get; set; }

    public int IdContrato { get; set; }

    public int AnioPeriodo { get; set; }

    public int MesPeriodo { get; set; }

    public DateOnly? FechaEmision { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public decimal? TotalDeuda { get; set; }

    public decimal? TotalPagado { get; set; }

    /// <summary>
    /// PENDIENTE, PARCIAL, PAGADO, ANULADO
    /// </summary>
    public string? EstadoPago { get; set; }

    public int? IdUsuarioGenerador { get; set; }

    public virtual ICollection<DeudaDetalle> DeudaDetalles { get; set; } = new List<DeudaDetalle>();

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual Usuario? IdUsuarioGeneradorNavigation { get; set; }

    public virtual ICollection<TransaccionPago> TransaccionPagos { get; set; } = new List<TransaccionPago>();
}
