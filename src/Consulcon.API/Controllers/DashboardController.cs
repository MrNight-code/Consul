using Consulcon.Application.Interfaces.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Consulcon.API.Controllers;

public class DashboardController(IDashboardMetricsService dashboardMetricsService) : BaseController
{
    /// <summary>
    /// Obtiene los contadores del dashboard para el condominio en contexto.
    /// Incluye: total de unidades, unidades en mora, total cobrado en el mes actual.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetContadores() 
        => HandleResult(await dashboardMetricsService.ObtenerMetricasAsync(CondominioId));

    /// <summary>
    /// Refresca los contadores del dashboard (fuerza recálculo de datos actuales).
    /// </summary>
    [HttpPost("refrescar")]
    public async Task<IActionResult> Refrescar() 
        => HandleResult(await dashboardMetricsService.ObtenerMetricasAsync(CondominioId));

    /// <summary>
    /// Obtiene los gastos agrupados por categoría para el gráfico.
    /// </summary>
    [HttpGet("gastos-por-categoria")]
    public async Task<IActionResult> GetGastosPorCategoria([FromQuery] int mes, [FromQuery] int anio)
        => HandleResult(await dashboardMetricsService.GetExpensesByCategoryAsync(CondominioId, mes, anio));
}
