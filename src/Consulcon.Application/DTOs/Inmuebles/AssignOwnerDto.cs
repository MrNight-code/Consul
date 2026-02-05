using System.ComponentModel.DataAnnotations;

namespace Consulcon.Application.DTOs.Inmuebles;

/// <summary>
/// DTO for assigning a new owner to a property.
/// </summary>
public class AssignOwnerDto
{
    /// <summary>
    /// ID of the property to assign ownership.
    /// </summary>
    [Required]
    public int PropiedadId { get; set; }

    /// <summary>
    /// ID of the new owner (Persona).
    /// </summary>
    [Required]
    public int NuevoDuenoId { get; set; }

    /// <summary>
    /// Start date of the new ownership.
    /// </summary>
    [Required]
    public DateOnly FechaInicio { get; set; }

    /// <summary>
    /// Optional observations about the ownership change.
    /// </summary>
    public string? Observaciones { get; set; }
}
