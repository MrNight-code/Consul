namespace Consulcon.Domain.Entities.Contabilidad;

/// Extensión de la clase Egreso para soportar el rastro de auditoría 

public partial class Egreso
{
    public bool EstaAnulado { get; set; } = false;

    public string? MotivoAnulacion { get; set; }

    public DateTime? FechaAnulacion { get; set; }

    public int? IdUsuarioAnulacion { get; set; }
}