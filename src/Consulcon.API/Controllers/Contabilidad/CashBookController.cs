using Consulcon.Application.DTOs.Contabilidad.CashBook;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Consulcon.API.Controllers.Contabilidad;

public class CashBookController(ICashBookService cashBookService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetCashBook([FromQuery] CashBookQuery query) 
        => HandleResult(await cashBookService.GetCashBookAsync(query));

    /// <summary>
    /// Exports the Cash Book report to an Excel file.
    /// </summary>
    /// <param name="query">Query parameters for filtering.</param>
    /// <param name="excelService">Injected Excel service.</param>
    /// <returns>An Excel (.xlsx) file containing the cash book entries.</returns>
    [HttpGet("export")]
    public async Task<IActionResult> ExportCashBook([FromQuery] CashBookQuery query, [FromServices] IExcelService excelService)
    {
        // Override pagination to fetch all entries for the export
        query.Page = 1;
        query.PageSize = int.MaxValue;

        var result = await cashBookService.GetCashBookAsync(query);
        
        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        // We export the Entries list. 
        // Note: The InitialBalance and FinalBalance are in the parent result, 
        // but for a raw tabular export, the Entries themselves are best.
        var dataList = result.Value.Entries.ToList();

        var fileContent = excelService.GenerateExcel(dataList, "Libro de Caja");
        
        return File(
            fileContent, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"Libro_Caja_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        );
    }
}
