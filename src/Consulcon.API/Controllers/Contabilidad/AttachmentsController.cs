using Consulcon.Application.DTOs.Contabilidad.Attachments;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

public class AttachmentsController(IExpenseAttachmentService attachmentService) : BaseController
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttachment(Guid id)
    {
        var result = await attachmentService.GetAttachmentAsync(id, CondominioId);
        
        if (result.IsFailure)
        {
            if (result.Error.Contains("no tiene permisos") || result.Error.Contains("Unauthorized"))
                return Forbid();
            
            return result.Error.Contains("no encontrado") ? NotFound(result.Error) : BadRequest(result.Error);
        }

        var file = result.Value;
        return File(file.FileStream, file.ContentType, file.FileName);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAttachments([FromQuery] AttachmentFilterDto filter)
    {
        var result = await attachmentService.GetAllAttachmentsAsync(filter);
        
        if (result.IsFailure) return BadRequest(result.Error);

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