using System.ComponentModel.DataAnnotations;

namespace Consulcon.Application.DTOs.Inmuebles;

/// <summary>
/// DTO for assigning a participant (Owner, Tenant, etc.) to a property.
/// Unified DTO replacing AssignOwnerDto and AssignTenantDto.
/// </summary>
public class AssignParticipantDto
{
    /// <summary>
    /// ID of the property.
    /// </summary>
    [Required]
    public int PropiedadId { get; set; }

    /// <summary>
    /// ID of the person.
    /// </summary>
    [Required]
    public int PersonaId { get; set; }

    /// <summary>
    /// Role in the contract (e.g., "PROPIETARIO", "INQUILINO").
    /// </summary>
    [Required]
    public string Rol { get; set; } = null!;

    /// <summary>
    /// Optional: ID of the specific contract to attach to.
    /// If null, the system will look for the currently active contract.
    /// </summary>
    public int? ContratoId { get; set; }

    /// <summary>
    /// Start date of the assignment.
    /// </summary>
    [Required]
    public DateOnly FechaInicio { get; set; }

    /// <summary>
    /// Optional: End date of the assignment.
    /// </summary>
    public DateOnly? FechaFin { get; set; }

    /// <summary>
    /// Optional observations.
    /// </summary>
    public string? Observaciones { get; set; }
}
