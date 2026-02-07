using Consulcon.Application.DTOs.Contabilidad.Expenses;
using Consulcon.Domain.Common;
using System.Threading.Tasks;

namespace Consulcon.Application.Interfaces.Contabilidad
{
    public interface IExpenseService
    {
        Task<Result<int>> RegisterExpenseAsync(RegisterExpenseCommand cmd, int userId);
    }
}
