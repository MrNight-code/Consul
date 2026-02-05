using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Interfaces.Inmuebles;

/// <summary>
/// Service interface for ownership assignment and history operations.
/// </summary>
public interface IOwnershipService
{
    /// <summary>
    /// Assigns a new owner to a property, closing the previous ownership record.
    /// </summary>
    /// <param name="dto">The assignment details.</param>
    /// <returns>The new ownership record.</returns>
    Task<Result<OwnershipHistoryDto>> AssignOwnerAsync(AssignOwnerDto dto);

    /// <summary>
    /// Gets the complete ownership history for a property.
    /// </summary>
    /// <param name="propiedadId">The property ID.</param>
    /// <returns>Chronological list of ownership records.</returns>
    Task<Result<List<OwnershipHistoryDto>>> GetOwnershipHistoryAsync(int propiedadId);
}
