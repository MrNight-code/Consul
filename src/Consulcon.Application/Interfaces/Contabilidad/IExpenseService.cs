using Consulcon.Application.DTOs.Contabilidad.Expenses;
using Consulcon.Domain.Common;
using System.Threading.Tasks;

namespace Consulcon.Application.Interfaces.Contabilidad
{
    public interface IExpenseService
    {
        Task<Result<int>> RegisterExpenseAsync(RegisterExpenseCommand cmd, int userId);
        Task<Result<PagedResult<EgresoDto>>> GetPagedAsync(int idCondominio, PaginationParams parameters);
        Task<Result<System.Collections.Generic.List<ExpensaDistribucionDto>>> SimularCalculoExpensasAsync(ExpensaSimulacionRequestDto request);
        Task<Result<System.Collections.Generic.List<SaldoUnidadDto>>> ObtenerSaldosUnidadesAsync(int fkCondominio);
        Task<Result<ConciliacionExpensaDto>> ConciliarExpensasAsync(ConciliarExpensasRequestDto request);
        Task<Result<GenerarDeudaResponseDto>> GenerarDeudaExpensasAsync(GenerarDeudaRequestDto request, int userId);
    }
}
