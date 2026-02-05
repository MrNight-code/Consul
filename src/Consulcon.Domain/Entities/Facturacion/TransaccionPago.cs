using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Facturacion;

public partial class TransaccionPago
{
    /// <summary>
    /// Antes: cuota
    /// </summary>
    public int IdPago { get; set; }

    /// <summary>
    /// Pago especifico de una deuda
    /// </summary>
    public int IdDeuda { get; set; }

    public int IdPersonaPagador { get; set; }

    public int IdBancoDestino { get; set; }

    public int IdFormaPago { get; set; }

    public DateTime? FechaPago { get; set; }

    public decimal MontoAbonado { get; set; }

    public decimal? TipoCambio { get; set; }

    public string? NroComprobanteBanco { get; set; }

    /// <summary>
    /// CONFIRMADO, RECHAZADO
    /// </summary>
    public string? Estado { get; set; }

    public string? Observaciones { get; set; }

    /// <summary>
    /// URL/Path local del recibo generado (PDF).
    /// </summary>
    public string? ReciboUrl { get; set; }

    /// <summary>
    /// Fecha y hora inmutable de generación del recibo (Server Time).
    /// </summary>
    public DateTime? FechaRecibo { get; set; }

    public virtual ICollection<AsientoContable> AsientoContables { get; set; } = [];

    public virtual Banco IdBancoDestinoNavigation { get; set; } = null!;

    public virtual DeudaCabecera IdDeudaNavigation { get; set; } = null!;

    public virtual FormaPago IdFormaPagoNavigation { get; set; } = null!;

    public virtual Persona IdPersonaPagadorNavigation { get; set; } = null!;
}
