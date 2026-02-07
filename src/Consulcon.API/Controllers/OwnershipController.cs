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
    /// Assigns a participant (Owner, Tenant, etc.) to a property.
    /// </summary>
    /// <param name="dto">The assignment details.</param>
    /// <returns>The created assignment or ownership record.</returns>
    [HttpPost("assign-participant")]
    public async Task<IActionResult> AssignParticipant([FromBody] AssignParticipantDto dto)
    {
        var result = await _ownershipService.AssignParticipantAsync(dto);

        if (result.IsFailure)
        {
            return BadRequest(new { IsSuccess = false, result.Error });
        }

        return Ok(new { IsSuccess = true, Data = result.Value, Message = "Participante asignado exitosamente." });
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
