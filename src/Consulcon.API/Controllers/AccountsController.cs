using Consulcon.Application.DTOs;
using Consulcon.Application.Interfaces;
using Consulcon.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers;

public class AccountsController(IAccountService service) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true) 
        => HandleResult(await service.GetAllAccountsAsync(activeOnly));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) 
        => HandleResult(await service.GetAccountByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AccountDto dto) 
        => HandleResult(await service.CreateAccountAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AccountDto dto) 
        => HandleResult(await service.UpdateAccountAsync(id, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) 
        => HandleResult(await service.DeleteAccountAsync(id));

    [HttpGet("{id}/balance-history")]
    public async Task<IActionResult> GetBalanceHistory(int id, [FromQuery] DateTime? from, [FromQuery] DateTime? to) 
        => HandleResult(await service.GetBalanceHistoryAsync(id, from, to));

    [HttpGet("{accountId}/transacciones/paged")]
    public async Task<IActionResult> GetPagedTransactions(int accountId, [FromQuery] PaginationParams parameters) 
        => HandleResult(await service.GetPagedTransactionHistoryAsync(accountId, parameters));
}