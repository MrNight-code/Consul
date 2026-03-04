using System.Threading.Tasks;
using Consulcon.Application.DTOs.Contabilidad.Attachments;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Mvc;
using Consulcon.Domain.Common;

namespace Consulcon.API.Controllers.Contabilidad;

[ApiController]
[Route("api/expenses")]
public class ExpensesController(IExpenseAttachmentService attachmentService, IExpenseService expenseService) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> RegisterExpense([FromBody] Consulcon.Application.DTOs.Contabilidad.Expenses.RegisterExpenseCommand cmd)
    {
        if (UserId == 0) return Unauthorized("User ID not found in token.");

        return HandleResult(await expenseService.RegisterExpenseAsync(cmd, UserId));
    }

    [HttpPost("{id}/attachments")]
    public async Task<IActionResult> UploadAttachment(int id, [FromForm] UploadAttachmentDto dto)
    {
        int currentUserId = UserId > 0 ? UserId : 1; 
        
        return HandleResult(await attachmentService.UploadAttachmentAsync(id, dto, currentUserId));
    }

    [HttpGet("{id}/attachments")]
    public async Task<IActionResult> GetAttachments(int id)
    {
        return HandleResult(await attachmentService.GetAttachmentsByExpenseIdAsync(id));
    }

    [HttpGet("condominio/{condominioId}")]
    public async Task<IActionResult> GetAll(int condominioId, [FromQuery] PaginationParams parameters)
    {
        return HandleResult(await expenseService.GetPagedAsync(condominioId, parameters));
    }
}