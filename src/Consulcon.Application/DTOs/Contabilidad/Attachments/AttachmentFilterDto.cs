namespace Consulcon.Application.DTOs.Contabilidad.Attachments;

public class AttachmentFilterDto
{
    public int? ExpenseId { get; set; }
    public DateTime? UploadedFrom { get; set; }
    public DateTime? UploadedTo { get; set; }
    public string? ContentType { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
