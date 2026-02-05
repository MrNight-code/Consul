using System.Threading.Tasks;
using Consulcon.Application.DTOs.Financiero;
using Consulcon.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers.Financiero;

[ApiController]
[Route("api/[controller]")] // api/FinancialConfig
public class FinancialConfigController : ControllerBase
{
    private readonly IFinancialConfigService _service;

    public FinancialConfigController(IFinancialConfigService service)
    {
        _service = service;
    }

    // --- Penalties / Financial Config ---

    [HttpGet("penalties/{condominiumId}")]
    public async Task<IActionResult> GetFinancialConfig(int condominiumId)
    {
        var result = await _service.GetFinancialConfigAsync(condominiumId);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPut("penalties/{condominiumId}")]
    public async Task<IActionResult> UpdateFinancialConfig(int condominiumId, [FromBody] UpdateFinancialConfigDto dto)
    {
        var result = await _service.UpdateFinancialConfigAsync(condominiumId, dto);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(new { IsSuccess = true, Message = "Configuracion actualizada correctamente." });
    }

    // --- Charge Concepts ---

    [HttpGet("concepts/{condominiumId}")]
    public async Task<IActionResult> GetConcepts(int condominiumId)
    {
        var result = await _service.GetChargeConceptsAsync(condominiumId);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpPost("concepts/{condominiumId}")]
    public async Task<IActionResult> CreateConcept(int condominiumId, [FromBody] CreateChargeConceptDto dto)
    {
        var result = await _service.CreateChargeConceptAsync(condominiumId, dto);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetConcepts), new { condominiumId }, new { Id = result.Value });
    }

    [HttpPut("concepts/{id}")]
    public async Task<IActionResult> UpdateConcept(int id, [FromBody] UpdateChargeConceptDto dto)
    {
        var result = await _service.UpdateChargeConceptAsync(id, dto);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(new { IsSuccess = true, Message = "Concepto actualizado." });
    }

    [HttpDelete("concepts/{id}")]
    public async Task<IActionResult> DeleteConcept(int id)
    {
        var result = await _service.DeleteChargeConceptAsync(id);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(new { IsSuccess = true, Message = "Concepto eliminado (Soft Delete)." });
    }
}
