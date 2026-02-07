using System;

namespace Consulcon.Application.DTOs.Contabilidad.Attachments;

public class ExpenseAttachmentDto
{
    public Guid Id { get; set; }
    public int ExpenseId { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
    public string? DownloadUrl { get; set; }
}
