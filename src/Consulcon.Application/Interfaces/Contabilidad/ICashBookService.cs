using Consulcon.Application.DTOs.Contabilidad.CashBook;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Interfaces.Contabilidad;

/// <summary>
/// Service interface for Cash Book operations.
/// </summary>
public interface ICashBookService
{
    /// <summary>
    /// Generates the cash book report consolidating incomes and expenses.
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination.</param>
    /// <returns>Paginated cash book result with running balances.</returns>
    Task<Result<CashBookResultDto>> GetCashBookAsync(CashBookQuery query);
}
