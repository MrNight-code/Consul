using System.IO;
using System.Threading.Tasks;
using Consulcon.Domain.Entities.Contabilidad;

namespace Consulcon.Application.Interfaces.Facturacion;

public interface IExpenseReceiptGenerationService
{
    Task<Stream> GenerateReceiptAsync(int egresoId);
}
