using Consulcon.Application.DTOs;
using Consulcon.Application.Interfaces;
using Consulcon.Domain.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Consulcon.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController(IAccountService service) : BaseController
    {

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true)
        {
            return HandleResult(await service.GetAllAccountsAsync(activeOnly));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await service.GetAccountByIdAsync(id);
            if (!result.IsSuccess) return NotFound(result.Error);
            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountDto dto)
        {
            var result = await service.CreateAccountAsync(dto);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AccountDto dto)
        {
            var result = await service.UpdateAccountAsync(id, dto);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return Ok(new { IsSuccess = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAccountAsync(id);
            if (!result.IsSuccess) return BadRequest(result.Error); // 400 Bad Request for Integrity violation
            return Ok(new { IsSuccess = true });
        }

        [HttpGet("{id}/balance-history")]
        public async Task<IActionResult> GetBalanceHistory(int id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var result = await service.GetBalanceHistoryAsync(id, from, to);
            
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("{accountId}/transacciones/paged")]
        public async Task<IActionResult> GetPagedTransactions(int accountId, [FromQuery] PaginationParams parameters)
        {
            var result = await service.GetPagedTransactionHistoryAsync(accountId, parameters);
            
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
