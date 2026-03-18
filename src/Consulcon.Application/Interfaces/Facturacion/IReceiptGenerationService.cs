using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Facturacion;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Consulcon.Application.Interfaces.Facturacion;

public interface IReceiptGenerationService
{
    Task<TransaccionPago> GenerateReceiptAsync(int transaccionId);
    Task<PagedResult<ReceiptDto>> GetGeneratedReceiptsAsync(PaginationParams parameters, string? medio = null, int? propiedadId = null);
    Task<Stream> GetBatchZipAsync(int mes, int anio);
    Task<byte[]> GenerateBatchReceiptsPdfAsync(BatchReceiptRequestDto request);
}
