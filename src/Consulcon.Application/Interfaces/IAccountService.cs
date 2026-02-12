using Consulcon.Application.DTOs;
using Consulcon.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consulcon.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Result<List<AccountDto>>> GetAllAccountsAsync(bool activeOnly = true);
        Task<Result<AccountDto>> GetAccountByIdAsync(int id);
        Task<Result<int>> CreateAccountAsync(AccountDto accountDto);
        Task<Result<bool>> UpdateAccountAsync(int id, AccountDto accountDto);
        Task<Result<bool>> DeleteAccountAsync(int id);
        Task<Result<IEnumerable<BalanceHistoryDto>>> GetBalanceHistoryAsync(int id, DateTime? from, DateTime? to);
    }
}
