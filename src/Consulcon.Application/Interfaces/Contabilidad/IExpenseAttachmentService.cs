using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Consulcon.Application.DTOs.Contabilidad.Attachments;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Interfaces.Contabilidad;

public interface IExpenseAttachmentService
{
    Task<Result<ExpenseAttachmentDto>> UploadAttachmentAsync(int expenseId, UploadAttachmentDto dto, string username);
    Task<Result<(Stream FileStream, string ContentType, string FileName)>> GetAttachmentAsync(Guid attachmentId, int requestTenantId);
    Task<Result<List<ExpenseAttachmentDto>>> GetAttachmentsByExpenseIdAsync(int expenseId);
    Task<Result<(List<ExpenseAttachmentDto> Items, int TotalCount)>> GetAllAttachmentsAsync(AttachmentFilterDto filter);
}
