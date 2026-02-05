namespace Consulcon.Application.DTOs.Inmuebles;

/// <summary>
/// DTO representing an ownership record in the history.
/// </summary>
public class OwnershipHistoryDto
{
    /// <summary>
    /// ID of the contract associated with this ownership.
    /// </summary>
    public int ContratoId { get; set; }

    /// <summary>
    /// ID of the owner (Persona).
    /// </summary>
    public int PersonaId { get; set; }

    /// <summary>
    /// Full name of the owner.
    /// </summary>
    public string NombrePersona { get; set; } = null!;

    /// <summary>
    /// Start date of the ownership period.
    /// </summary>
    public DateOnly FechaInicio { get; set; }

    /// <summary>
    /// End date of the ownership period (null if current owner).
    /// </summary>
    public DateOnly? FechaFin { get; set; }

    /// <summary>
    /// Indicates if this is the current active ownership.
    /// </summary>
    public bool EsVigente { get; set; }
}
