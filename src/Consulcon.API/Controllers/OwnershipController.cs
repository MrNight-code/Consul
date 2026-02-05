using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers;

/// <summary>
/// Controller for property ownership assignment and history operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OwnershipController(IOwnershipService ownershipService) : ControllerBase
{
    private readonly IOwnershipService _ownershipService = ownershipService;

    /// <summary>
    /// Assigns a new owner to a property. Closes the previous ownership record transactionally.
    /// </summary>
    /// <param name="dto">The assignment details.</param>
    /// <returns>The new ownership record.</returns>
    [HttpPost("assign-owner")]
    public async Task<IActionResult> AssignOwner([FromBody] AssignOwnerDto dto)
    {
        var result = await _ownershipService.AssignOwnerAsync(dto);

        if (result.IsFailure)
        {
            return BadRequest(new { IsSuccess = false, result.Error });
        }

        return Ok(new { IsSuccess = true, Data = result.Value, Message = "Propietario asignado exitosamente." });
    }

    /// <summary>
    /// Gets the complete chronological ownership history for a property.
    /// </summary>
    /// <param name="propiedadId">The property ID.</param>
    /// <returns>List of ownership records.</returns>
    [HttpGet("history/{propiedadId}")]
    public async Task<IActionResult> GetOwnershipHistory(int propiedadId)
    {
        var result = await _ownershipService.GetOwnershipHistoryAsync(propiedadId);

        if (result.IsFailure)
        {
            return NotFound(new { IsSuccess = false, result.Error });
        }

        return Ok(new { IsSuccess = true, Data = result.Value });
    }
}
