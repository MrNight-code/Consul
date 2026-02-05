using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Contabilidad;

public partial class Egreso
{
    public int IdEgreso { get; set; }

    public int IdCondominio { get; set; }

    /// <summary>
    /// Opcional
    /// </summary>
    public int? IdProveedor { get; set; }

    /// <summary>
    /// Opcional
    /// </summary>
    public int? IdPersonaBeneficiario { get; set; }

    public int IdAutorizacion { get; set; }

    public int IdBancoOrigen { get; set; }

    public int IdFormaPago { get; set; }

    public string Concepto { get; set; } = null!;

    public decimal MontoTotal { get; set; }

    public DateTime? FechaEgreso { get; set; }

    public string? NroFacturaProveedor { get; set; }

    public int IdUsuarioRegistro { get; set; }

    public virtual ICollection<AsientoContable> AsientoContables { get; set; } = new List<AsientoContable>();

    public virtual AutorizacionGasto IdAutorizacionNavigation { get; set; } = null!;

    public virtual Banco IdBancoOrigenNavigation { get; set; } = null!;

    public virtual Condominio IdCondominioNavigation { get; set; } = null!;

    public virtual FormaPago IdFormaPagoNavigation { get; set; } = null!;

    public virtual Persona? IdPersonaBeneficiarioNavigation { get; set; }

    public virtual Proveedor? IdProveedorNavigation { get; set; }

    public virtual Usuario IdUsuarioRegistroNavigation { get; set; } = null!;
}
