using Consulcon.Application.DTOs.Financiero;
using Consulcon.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Financiero;

public class FinancialConfigController(IFinancialConfigService service) : BaseController
{
    [HttpGet("penalties")]
    public async Task<IActionResult> GetFinancialConfig() 
        => HandleResult(await service.GetFinancialConfigAsync(CondominioId));

    [HttpPut("penalties")]
    public async Task<IActionResult> UpdateFinancialConfig([FromBody] UpdateFinancialConfigDto dto) 
        => HandleResult(await service.UpdateFinancialConfigAsync(CondominioId, dto));

    [HttpGet("concepts")]
    public async Task<IActionResult> GetConcepts() 
        => HandleResult(await service.GetChargeConceptsAsync(CondominioId));

    [HttpPost("concepts")]
    public async Task<IActionResult> CreateConcept([FromBody] CreateChargeConceptDto dto) 
        => HandleResult(await service.CreateChargeConceptAsync(CondominioId, dto));

    [HttpPut("concepts/{id}")]
    public async Task<IActionResult> UpdateConcept(int id, [FromBody] UpdateChargeConceptDto dto) 
        => HandleResult(await service.UpdateChargeConceptAsync(id, dto));

    [HttpDelete("concepts/{id}")]
    public async Task<IActionResult> DeleteConcept(int id) 
        => HandleResult(await service.DeleteChargeConceptAsync(id));
}