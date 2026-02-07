using Consulcon.Application.DTOs.Contabilidad.CashBook;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Contabilidad;

/// <summary>
/// Controller for Cash Book (Libro de Caja) operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CashBookController(ICashBookService cashBookService) : ControllerBase
{
    /// <summary>
    /// Generates the Cash Book report consolidating incomes and expenses.
    /// </summary>
    /// <param name="query">Query parameters for filtering and pagination.</param>
    /// <returns>Paginated cash book with running balances.</returns>
    [HttpGet]
    public async Task<IActionResult> GetCashBook([FromQuery] CashBookQuery query)
    {
        var result = await cashBookService.GetCashBookAsync(query);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });
            
        return Ok(result.Value);
    }
}
