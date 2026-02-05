using Consulcon.Application.DTOs.Dashboard;
using Consulcon.Domain.Common;
using System.Threading.Tasks;

namespace Consulcon.Application.Interfaces.Dashboard;

/// <summary>
/// Servicio para obtener y calcular métricas financieras y operativas del Dashboard principal.
/// Reemplaza la lógica simplificada anterior con cálculos reales de Eficiencia, Mora y Cash Flow.
/// </summary>
public interface IDashboardMetricsService
{
    /// <summary>
    /// Calcula los contadores del dashboard para un condominio específico.
    /// Realiza agregaciones en tiempo real o casi real sobre las tablas transaccionales.
    /// </summary>
    /// <param name="condominioId">ID del condominio (Tenant).</param>
    /// <returns>DTO con los contadores financieros y operativos.</returns>
    Task<Result<DashboardCountersDto>> ObtenerMetricasAsync(int condominioId);
}
