using Consulcon.Application.Interfaces.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers;

/// Controller para obtener contadores e información agregada del Dashboard principal.

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IDashboardMetricsService dashboardMetricsService) : ControllerBase
{
    private readonly IDashboardMetricsService _dashboardMetricsService = dashboardMetricsService;

    /// Obtiene los contadores del dashboard para un condominio.
    /// Incluye: total de unidades, unidades en mora, total cobrado en el mes actual.
    [HttpGet("{condominioId}")]
    public async Task<IActionResult> GetContadores(int condominioId)
    {
        var result = await _dashboardMetricsService.ObtenerMetricasAsync(condominioId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        // Si no encontró el condominio, retornar 404
        if (result.Error?.Contains("no encontrado") == true)
        {
            return NotFound(new { Message = result.Error });
        }

        // Otros errores retornan 400
        return BadRequest(new { Message = result.Error });
    }

    /// Refresca los contadores del dashboard (fuerza recálculo de datos actuales).
    /// Útil cuando el usuario presiona un botón "Refrescar" en el frontend.
    [HttpPost("{condominioId}/refrescar")]
    public async Task<IActionResult> Refrescar(int condominioId)
    {
        var result = await _dashboardMetricsService.ObtenerMetricasAsync(condominioId);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        // Si no encontró el condominio, retornar 404
        if (result.Error?.Contains("no encontrado") == true)
        {
            return NotFound(new { Message = result.Error });
        }

        // Otros errores retornan 400
        return BadRequest(new { Message = result.Error });
    }
}
