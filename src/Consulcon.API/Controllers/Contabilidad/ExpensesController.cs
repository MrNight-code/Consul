using Consulcon.Application.DTOs.Contabilidad.Attachments;
using Consulcon.Application.DTOs.Contabilidad.Expenses;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

public class ExpensesController(
    IExpenseAttachmentService attachmentService, 
    IExpenseService expenseService) : BaseController
{
    public async Task<IActionResult> RegisterExpense([FromBody] RegisterExpenseCommand cmd)
        => HandleResult(await expenseService.RegisterExpenseAsync(cmd, UserId));

    [HttpPost("simular")]
    public async Task<IActionResult> SimularCalculo([FromBody] Consulcon.Application.DTOs.Contabilidad.Expenses.ExpensaSimulacionRequestDto request)
    {
        return HandleResult(await expenseService.SimularCalculoExpensasAsync(request));
    }

    [HttpPost("{id}/attachments")]
    public async Task<IActionResult> UploadAttachment(int id, [FromForm] UploadAttachmentDto dto)
    {
        var userNameClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name)
            ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName)
            ?? User.FindFirst("unique_name");

        var uploadedBy = userNameClaim?.Value;
        if (string.IsNullOrEmpty(uploadedBy))
        {
            return Unauthorized(new { Message = "No se pudo identificar al usuario intentando subir el archivo." });
        }

        return HandleResult(await attachmentService.UploadAttachmentAsync(id, dto, uploadedBy));
    }

    [HttpGet("{id}/attachments")]
    public async Task<IActionResult> GetAttachments(int id) 
        => HandleResult(await attachmentService.GetAttachmentsByExpenseIdAsync(id));

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationParams parameters) 
        => HandleResult(await expenseService.GetPagedAsync(CondominioId, parameters));

    [HttpGet("saldos-unidades")]
    public async Task<IActionResult> ObtenerSaldosUnidades()
        => HandleResult(await expenseService.ObtenerSaldosUnidadesAsync(CondominioId));

    [HttpPost("conciliar")]
    public async Task<IActionResult> ConciliarExpensas([FromBody] Consulcon.Application.DTOs.Contabilidad.Expenses.ConciliarExpensasRequestDto request)
        => HandleResult(await expenseService.ConciliarExpensasAsync(request));

    [HttpPost("generar-deuda")]
    public async Task<IActionResult> GenerarDeudaExpensas([FromBody] Consulcon.Application.DTOs.Contabilidad.Expenses.GenerarDeudaRequestDto request)
        => HandleResult(await expenseService.GenerarDeudaExpensasAsync(request, UserId));
}
