using Consulcon.Application.DTOs.Dashboard;
using Consulcon.Application.Interfaces.Dashboard;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Interfaces;
using Consulcon.Domain.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Consulcon.Application.Services.Dashboard;

public class DashboardMetricsService : IDashboardMetricsService
{
    private readonly IRepository<Propiedad> _propiedadRepository;
    private readonly IRepository<Condominio> _condominioRepository;
    private readonly IRepository<DeudaCabecera> _deudaRepository;
    private readonly IRepository<TransaccionPago> _transaccionPagoRepository;
    private readonly IRepository<Egreso> _egresoRepository;

    public DashboardMetricsService(
        IRepository<Propiedad> propiedadRepository,
        IRepository<Condominio> condominioRepository,
        IRepository<DeudaCabecera> deudaRepository,
        IRepository<TransaccionPago> transaccionPagoRepository,
        IRepository<Egreso> egresoRepository)
    {
        _propiedadRepository = propiedadRepository;
        _condominioRepository = condominioRepository;
        _deudaRepository = deudaRepository;
        _transaccionPagoRepository = transaccionPagoRepository;
        _egresoRepository = egresoRepository;
    }

    public async Task<Result<DashboardCountersDto>> ObtenerMetricasAsync(int condominioId)
    {
        try
        {
            // 1. Validar Condominio
            var condominio = await _condominioRepository.GetByIdAsync(condominioId);
            if (condominio == null)
            {
                return Result.Fail<DashboardCountersDto>($"Condominio con ID {condominioId} no encontrado");
            }

            // --- Cálculos de Fechas (Mes Actual) ---
            var ahora = DateTime.Now;
            var primerDiaMes = new DateTime(ahora.Year, ahora.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            // 2. Métricas de Unidades (Operativo)
            // Nota: Optimizamos usando un solo query si es posible, o dos filtrados.
            var propiedades = await _propiedadRepository.FindAsync(
                p => p.IdManzanoNavigation.IdCondominio == condominioId && p.Activo == true,
                includeProperties: "IdManzanoNavigation"
            );
            
            var totalUnidades = propiedades.Count();
            var unidadesEnMora = propiedades.Count(p => p.SaldoDeudor > 0);
            var moraHistorica = propiedades.Sum(p => p.SaldoDeudor); // Total Mora Histórica

            // 3. Ingresos (Cobranza del Mes)
            var transaccionesDelMes = await _transaccionPagoRepository.FindAsync(
                p => p.FechaPago >= primerDiaMes && p.FechaPago <= ultimoDiaMes,
                includeProperties: "IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation.IdManzanoNavigation"
            );
            
            // Filtrar en memoria por restricción del repositorio genérico, pero el dataset mensual es manejable.
            // Idealmente el repositorio debería permitir filtrar por propiedades anidadas en el LINQ expression.
            var totalCobradoMesActual = transaccionesDelMes
                .Where(p => p.IdDeudaNavigation?.IdContratoNavigation?.IdPropiedadNavigation?.IdManzanoNavigation?.IdCondominio == condominioId)
                .Sum(p => p.MontoAbonado);

            // 4. Egresos (Gastos del Mes) - Para Cash Flow
            var egresosDelMes = await _egresoRepository.FindAsync(
                e => e.IdCondominio == condominioId &&
                     e.FechaEgreso >= primerDiaMes && 
                     e.FechaEgreso <= ultimoDiaMes
            );

            var totalEgresosMesActual = egresosDelMes.Sum(e => e.MontoTotal);

            // 5. Eficiencia de Cobranza
            // Eficiencia = (Cobrado del Mes / Deuda Generada en el Mes) * 100
            var deudasDelMes = await _deudaRepository.FindAsync(
                d => d.AnioPeriodo == ahora.Year &&
                     d.MesPeriodo == ahora.Month,
                includeProperties: "IdContratoNavigation.IdPropiedadNavigation.IdManzanoNavigation"
            );

            var totalDeudaGeneradaMes = deudasDelMes
                .Where(d => d.IdContratoNavigation?.IdPropiedadNavigation?.IdManzanoNavigation?.IdCondominio == condominioId)
                .Sum(d => d.TotalDeuda ?? 0);

            var porcentajeCobranza = totalDeudaGeneradaMes > 0 
                ? Math.Round((totalCobradoMesActual / totalDeudaGeneradaMes) * 100, 2)
                : 0;

            // 6. Construir DTO
            var dto = new DashboardCountersDto
            {
                TotalUnidades = totalUnidades,
                UnidadesEnMora = unidadesEnMora,
                TotalMoraHistorica = moraHistorica,
                
                TotalCobradoMesActual = totalCobradoMesActual,
                TotalEgresosMesActual = totalEgresosMesActual,
                CashFlowMesActual = totalCobradoMesActual - totalEgresosMesActual, // Cash Flow Simple
                
                PorcentajeCobranza = porcentajeCobranza,
                
                CondominioNombre = condominio.Nombre,
                UltimaActualizacion = DateTime.Now
            };

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result.Fail<DashboardCountersDto>($"Error calculando métricas financieras: {ex.Message}");
        }
    }
}
