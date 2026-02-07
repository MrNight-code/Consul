using Microsoft.AspNetCore.Http;

namespace Consulcon.Application.DTOs.Contabilidad.Attachments;

public class UploadAttachmentDto
{
    public IFormFile File { get; set; } = null!;
}
