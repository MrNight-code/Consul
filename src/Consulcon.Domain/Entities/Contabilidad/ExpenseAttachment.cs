using System;

namespace Consulcon.Domain.Entities.Contabilidad;

public class ExpenseAttachment
{
    public Guid Id { get; set; }
    public int EgresoId { get; set; }
    public string FileName { get; set; } = null!;
    public string StoredFileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public string StoragePath { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public int UploadedBy { get; set; }
    public int TenantId { get; set; }

    public virtual Egreso Egreso { get; set; } = null!;
}
