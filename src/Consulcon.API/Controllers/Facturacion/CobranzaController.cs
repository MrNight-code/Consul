using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Common;
using Consulcon.Application.Interfaces.Facturacion;
using Consulcon.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Consulcon.API.Controllers.Facturacion;

public class CobranzaController(ICobranzaService service) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> RegistrarCobranza([FromBody] CobranzaRequest request) 
        => HandleResult(await service.RegistrarCobranzaAsync(request));

    [HttpGet("{unitId}")]
    public async Task<IActionResult> ObtenerHistorial(int unitId) 
        => HandleResult(await service.ObtenerHistorialAsync(unitId));

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParams parameters) 
        => HandleResult(await service.GetPagedAsync(CondominioId, parameters));

    [HttpGet("{unitId}/export")]
    [Authorize]
    public async Task<IActionResult> ExportarHistorial(int unitId, [FromServices] IExcelService excelService)
    {
        var result = await service.ObtenerHistorialAsync(unitId);
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dataList = result.Value;

        var fileContent = excelService.GenerateExcel(dataList, $"Historial_{unitId}");
        
        return File(
            fileContent, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"Historial_Cobranzas_Unidad_{unitId}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        );
    }
}
