using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Facturacion;

public partial class ConfigAvisoCobranza
{
    /// <summary>
    /// Antes: confaviso
    /// </summary>
    public int IdConfig { get; set; }

    public int IdCondominio { get; set; }

    public string? TextoHeader { get; set; }

    public string? TextoFooter { get; set; }

    public int? DiasVencimientoDefecto { get; set; }

    public virtual Condominio IdCondominioNavigation { get; set; } = null!;
}
