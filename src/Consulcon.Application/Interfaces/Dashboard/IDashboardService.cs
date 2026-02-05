using Consulcon.Application.DTOs.Dashboard;

namespace Consulcon.Application.Interfaces.Dashboard;

/// Servicio para obtener contadores agregados del Dashboard principal.
/// Actúa como agregador de datos de múltiples servicios (Propiedad, Cobranza, Deuda).

public interface IDashboardService
{
    /// Obtiene los contadores del dashboard para un condominio específico.
    /// Incluye: total de unidades, unidades en mora, total cobrado en el mes actual.
    Task<Result<DashboardCountersDto>> ObtenerContadoresAsync(int condominioId);

    /// Refresca los contadores (fuerza recálculo de datos actuales).
    /// Mismo resultado que ObtenerContadoresAsync pero garantiza datos más frescos.
    Task<Result<DashboardCountersDto>> RefrescarContadoresAsync(int condominioId);
}
