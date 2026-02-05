using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Domain.Entities.Facturacion;
using System.Threading.Tasks;

namespace Consulcon.Application.Interfaces.Facturacion;

public interface IReceiptGenerationService
{
    Task<TransaccionPago> GenerateReceiptAsync(int transaccionId);
    Task<List<ReceiptDto>> GetGeneratedReceiptsAsync(ReceiptFilterDto filter);
}
