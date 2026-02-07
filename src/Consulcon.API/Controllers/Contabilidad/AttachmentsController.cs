using System;
using System.Threading.Tasks;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

[ApiController]
[Route("api/attachments")]
public class AttachmentsController(IExpenseAttachmentService attachmentService, ICurrentTenantService tenantService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttachment(Guid id)
    {
        // Try parsing TenantId, default to 0 if fails or null
        if (!int.TryParse(tenantService.TenantId, out int requestTenantId))
        {
             requestTenantId = 0;
        }

        var result = await attachmentService.GetAttachmentAsync(id, requestTenantId);
        if (result.IsFailure)
        {
            if (result.Error.Contains("no tiene permisos") || result.Error.Contains("Unauthorized"))
                return Forbid();
            return NotFound(result.Error);
        }

        var file = result.Value;
        return File(file.FileStream, file.ContentType, file.FileName);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAttachments([FromQuery] Application.DTOs.Contabilidad.Attachments.AttachmentFilterDto filter)
    {
        var result = await attachmentService.GetAllAttachmentsAsync(filter);
        if (result.IsFailure)
            return BadRequest(result.Error);

        var (items, totalCount) = result.Value;
        return Ok(new
        {
            items,
            totalCount,
            filter.PageNumber,
            filter.PageSize
        });
    }
}
