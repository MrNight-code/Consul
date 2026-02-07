using System.Threading.Tasks;
using System.Security.Claims; // Added
using Consulcon.Application.DTOs.Contabilidad.Attachments;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Application.Interfaces.Seguridad;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

[ApiController]
[Route("api/expenses")]
public class ExpensesController(IExpenseAttachmentService attachmentService, IExpenseService expenseService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RegisterExpense([FromBody] Consulcon.Application.DTOs.Contabilidad.Expenses.RegisterExpenseCommand cmd)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await expenseService.RegisterExpenseAsync(cmd, userId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value); // Returns IdEgreso
        }
        else
        {
            return BadRequest(result.Error);
        }
    }

    [HttpPost("{id}/attachments")]
    public async Task<IActionResult> UploadAttachment(int id, [FromForm] UploadAttachmentDto dto)
    {
        // TODO: Get real userId from identity service
        // var userId = _identityService.UserId; 
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        int userId = 1;
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int uid)) userId = uid;

        var result = await attachmentService.UploadAttachmentAsync(id, dto, userId);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
    
    [HttpGet("{id}/attachments")]
    public async Task<IActionResult> GetAttachments(int id)
    {
        var result = await attachmentService.GetAttachmentsByExpenseIdAsync(id);
        if (result.IsFailure)
             return BadRequest(result.Error);
             
        return Ok(result.Value);
    }
}
