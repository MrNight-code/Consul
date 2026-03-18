using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers;

public class OwnershipController(IOwnershipService ownershipService) : BaseController
{
    /// <summary>
    /// Assigns a participant (Owner, Tenant, etc.) to a property.
    /// </summary>
    [HttpPost("assign-participant")]
    public async Task<IActionResult> AssignParticipant([FromBody] AssignParticipantDto dto) 
        => HandleResult(await ownershipService.AssignParticipantAsync(dto));

    /// <summary>
    /// Gets the complete chronological ownership history for a property.
    /// </summary>
    [HttpGet("history/{propiedadId}")]
    public async Task<IActionResult> GetOwnershipHistory(int propiedadId) 
        => HandleResult(await ownershipService.GetOwnershipHistoryAsync(propiedadId));
}