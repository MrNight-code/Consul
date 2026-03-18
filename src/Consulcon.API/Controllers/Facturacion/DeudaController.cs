using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;
using Consulcon.Application.Interfaces.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Consulcon.API.Controllers.Facturacion;

public class DeudaController(IDeudaService service) : BaseController
{
    [HttpGet("pendiente")]
    public async Task<IActionResult> GetPending() 
        => HandleResult(await service.GetPendingAsync());

    [HttpGet("contrato/{contratoId}")]
    public async Task<IActionResult> GetByContrato(int contratoId) 
        => HandleResult(await service.GetByContratoAsync(contratoId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) 
        => HandleResult(await service.GetByIdAsync(id));

    [HttpPost("generar")]
    public async Task<IActionResult> Generate([FromBody] GenerateDeudaDto dto) 
        => HandleResult(await service.GenerateDeudaAsync(dto));

    [HttpGet("pendiente/export")]
    [Authorize]
    public async Task<IActionResult> ExportarDeudasPendientes([FromServices] IExcelService excelService)
    {
        var result = await service.GetPendingAsync();
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);

        var dataList = result.Value.ToList();

        var fileContent = excelService.GenerateExcel(dataList, "Deudas Pendientes");
        
        return File(
            fileContent, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"Deudas_Pendientes_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        );
    }
}
