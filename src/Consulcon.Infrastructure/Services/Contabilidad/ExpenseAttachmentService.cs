using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Consulcon.Application.DTOs.Contabilidad.Attachments;
using Consulcon.Application.Interfaces;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Infrastructure.Persistence;
using Consulcon.Infrastructure.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Consulcon.Infrastructure.Services.Contabilidad;

public class ExpenseAttachmentService(
    ConsulconDbContext context,
    IFileStorageStrategy storageStrategy,
    ILogger<ExpenseAttachmentService> logger) : IExpenseAttachmentService
{
    public async Task<Result<ExpenseAttachmentDto>> UploadAttachmentAsync(int expenseId, UploadAttachmentDto dto, string username)
    {
        var localUser = await context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);
        if (localUser == null)
            return Result.Fail<ExpenseAttachmentDto>($"El usuario '{username}' no existe en este condominio.");
        int userId = localUser.IdUsuario;
        
        var expense = await context.Egresos.FindAsync(expenseId);
        if (expense == null)
            return Result.Fail<ExpenseAttachmentDto>("El gasto especificado no existe.");

        // Validation
        if (dto.File == null || dto.File.Length == 0)
            return Result.Fail<ExpenseAttachmentDto>("El archivo es inválido o está vacío.");

        if (dto.File.Length > 5 * 1024 * 1024) // 5MB
            return Result.Fail<ExpenseAttachmentDto>("El archivo excede el tamaño máximo permitido de 5MB.");

        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
        if (!validExtensions.Contains(extension))
            return Result.Fail<ExpenseAttachmentDto>("Formato de archivo no permitido. Solo se aceptan JPG, PNG y PDF.");

        // TODO: Magic number validation could be added here for stricter security

        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var tenantId = expense.IdCondominio; // Assuming CondominioId maps to Tenant logic or folder structure
        // Note: expense has IdCondominio, and storage strategy asks for TenantId. Using IdCondominio as TenantId for storage.
        
        var year = expense.FechaEgreso?.Year ?? DateTime.Now.Year;

        try
        {
            string storagePath;
            using (var stream = dto.File.OpenReadStream())
            {
                storagePath = await storageStrategy.SaveFileAsync(stream, storedFileName, tenantId, year);
            }

            var attachment = new ExpenseAttachment
            {
                Id = Guid.NewGuid(),
                EgresoId = expenseId,
                FileName = dto.File.FileName,
                StoredFileName = storedFileName,
                ContentType = dto.File.ContentType,
                Size = dto.File.Length,
                StoragePath = storagePath,
                UploadedAt = DateTime.Now,
                UploadedBy = userId,
                TenantId = tenantId
            };

            context.ExpenseAttachments.Add(attachment);
            await context.SaveChangesAsync();

            return Result.Ok(new ExpenseAttachmentDto
            {
                Id = attachment.Id,
                ExpenseId = attachment.EgresoId,
                FileName = attachment.FileName,
                ContentType = attachment.ContentType,
                Size = attachment.Size,
                UploadedAt = attachment.UploadedAt
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading attachment for expense {ExpenseId}", expenseId);
            return Result.Fail<ExpenseAttachmentDto>("Ocurrió un error al subir el archivo.");
        }
    }

    public async Task<Result<(Stream FileStream, string ContentType, string FileName)>> GetAttachmentAsync(Guid attachmentId, int requestTenantId)
    {
        var attachment = await context.ExpenseAttachments.FindAsync(attachmentId);
        if (attachment == null)
            return Result.Fail<(Stream FileStream, string ContentType, string FileName)>("El archivo adjunto no existe.");

        // Security check
        if (attachment.TenantId != requestTenantId)
        {
            logger.LogWarning("Unauthorized access attempt to attachment {AttachmentId} by tenant {TenantId}", attachmentId, requestTenantId);
            return Result.Fail<(Stream FileStream, string ContentType, string FileName)>("No tiene permisos para acceder a este archivo.");
        }

        try
        {
            var stream = await storageStrategy.GetFileAsync(attachment.StoragePath);
            return Result.Ok((stream, attachment.ContentType, attachment.FileName));
        }
        catch (FileNotFoundException)
        {
            return Result.Fail<(Stream, string, string)>("El archivo físico no se encuentra en el servidor.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving attachment {AttachmentId}", attachmentId);
            return Result.Fail<(Stream, string, string)>("Ocurrió un error al recuperar el archivo.");
        }
    }

    public async Task<Result<List<ExpenseAttachmentDto>>> GetAttachmentsByExpenseIdAsync(int expenseId)
    {
        var attachments = await context.ExpenseAttachments
            .Where(a => a.EgresoId == expenseId)
            .Select(a => new ExpenseAttachmentDto
            {
                Id = a.Id,
                ExpenseId = a.EgresoId,
                FileName = a.FileName,
                ContentType = a.ContentType,
                Size = a.Size,
                UploadedAt = a.UploadedAt
            })
            .ToListAsync();

        return Result.Ok(attachments);
    }

    public async Task<Result<(List<ExpenseAttachmentDto> Items, int TotalCount)>> GetAllAttachmentsAsync(AttachmentFilterDto filter)
    {
        var query = context.ExpenseAttachments.AsQueryable();

        // Apply filters
        if (filter.ExpenseId.HasValue)
            query = query.Where(a => a.EgresoId == filter.ExpenseId.Value);

        if (filter.UploadedFrom.HasValue)
            query = query.Where(a => a.UploadedAt >= filter.UploadedFrom.Value);

        if (filter.UploadedTo.HasValue)
            query = query.Where(a => a.UploadedAt <= filter.UploadedTo.Value);

        if (!string.IsNullOrWhiteSpace(filter.ContentType))
            query = query.Where(a => a.ContentType.Contains(filter.ContentType));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.UploadedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(a => new ExpenseAttachmentDto
            {
                Id = a.Id,
                ExpenseId = a.EgresoId,
                FileName = a.FileName,
                ContentType = a.ContentType,
                Size = a.Size,
                UploadedAt = a.UploadedAt
            })
            .ToListAsync();

        return Result.Ok((items, totalCount));
    }
}
