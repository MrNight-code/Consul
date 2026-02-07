using System.ComponentModel.DataAnnotations;

namespace Consulcon.Application.DTOs.Inmuebles;

/// <summary>
/// DTO for assigning a tenant or resident to a property.
/// </summary>
public class AssignTenantDto
{
    /// <summary>
    /// ID of the property.
    /// </summary>
    [Required]
    public int PropiedadId { get; set; }

    /// <summary>
    /// ID of the person (Tenant/Resident).
    /// </summary>
    [Required]
    public int PersonaId { get; set; }

    /// <summary>
    /// Optional: ID of the specific contract to attach to.
    /// If null, the system will look for the currently active contract.
    /// </summary>
    public int? ContratoId { get; set; }

    /// <summary>
    /// Start date of the assignment (Fecha Alta).
    /// </summary>
    [Required]
    public DateOnly FechaInicio { get; set; }

    /// <summary>
    /// Optional: End date of the assignment (Fecha Baja).
    /// </summary>
    public DateOnly? FechaFin { get; set; }

    /// <summary>
    /// Optional observations.
    /// </summary>
    public string? Observaciones { get; set; }
}
