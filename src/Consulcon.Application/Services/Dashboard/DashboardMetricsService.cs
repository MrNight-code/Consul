using Consulcon.Application.DTOs.Dashboard;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Entities.Contratos;
using Consulcon.Domain.Entities.Reservas;
using Consulcon.Domain.Entities.Financiero;
using Consulcon.Domain.Entities;
using Consulcon.Domain.Interfaces;
using Consulcon.Domain.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Consulcon.Application.Interfaces.Dashboard;

namespace Consulcon.Application.Services.Dashboard;

public class DashboardMetricsService : IDashboardMetricsService
{
    private readonly IRepository<Propiedad> _propiedadRepository;
    private readonly IRepository<Condominio> _condominioRepository;
    private readonly IRepository<DeudaCabecera> _deudaRepository;
    private readonly IRepository<TransaccionPago> _transaccionPagoRepository;
    private readonly IRepository<Egreso> _egresoRepository;
    private readonly IRepository<Persona> _personaRepository;
    private readonly IRepository<Contrato> _contratoRepository;
    private readonly IRepository<Reserva> _reservaRepository;
    private readonly IRepository<FinancialConfig> _financialConfigRepository;

    public DashboardMetricsService(
        IRepository<Propiedad> propiedadRepository,
        IRepository<Condominio> condominioRepository,
        IRepository<DeudaCabecera> deudaRepository,
        IRepository<TransaccionPago> transaccionPagoRepository,
        IRepository<Egreso> egresoRepository,
        IRepository<Persona> personaRepository,
        IRepository<Contrato> contratoRepository,
        IRepository<Reserva> reservaRepository,
        IRepository<FinancialConfig> financialConfigRepository)
    {
        _propiedadRepository = propiedadRepository;
        _condominioRepository = condominioRepository;
        _deudaRepository = deudaRepository;
        _transaccionPagoRepository = transaccionPagoRepository;
        _egresoRepository = egresoRepository;
        _personaRepository = personaRepository;
        _contratoRepository = contratoRepository;
        _reservaRepository = reservaRepository;
        _financialConfigRepository = financialConfigRepository;
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
            var primerDiaSiguienteMes = primerDiaMes.AddMonths(1);

            // Fetch Financial Config for Grace Days if needed later, but keeping user's simplified logic for now
            // as they manually edited it. However, I will restore the one-shot metrics.

            // 2. Métricas de Unidades (Operativo)
            // Nota: Optimizamos usando un solo query si es posible, o dos filtrados.
            var propiedades = await _propiedadRepository.FindAsync(
                p => p.IdManzanoNavigation.IdCondominio == condominioId && p.Activo == true,
                includeProperties: "IdManzanoNavigation"
            );
            
            var propIds = propiedades.Select(p => p.IdPropiedad).ToList();
            var totalUnidades = propiedades.Count();
            var unidadesEnMora = propiedades.Count(p => p.SaldoDeudor > 0);
            var moraHistorica = propiedades.Sum(p => p.SaldoDeudor); // Total Mora Histórica

            // 3. Ingresos (Cobranza del Mes)
            var transaccionesDelMes = await _transaccionPagoRepository.FindAsync(
                p => p.FechaPago >= primerDiaMes && p.FechaPago < primerDiaSiguienteMes && p.Estado == "CONFIRMADO",
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
                     e.FechaEgreso < primerDiaSiguienteMes
            );
            var totalEgresosMesActual = egresosDelMes.Sum(e => e.MontoTotal);

            // 5. Eficiencia de Cobranza
            var deudasDelMes = await _deudaRepository.FindAsync(
                d => d.AnioPeriodo == ahora.Year && d.MesPeriodo == ahora.Month &&
                     propIds.Contains(d.IdContratoNavigation.IdPropiedad),
                includeProperties: "IdContratoNavigation"
            );

            var totalDeudaGeneradaMes = deudasDelMes.Sum(d => d.TotalDeuda ?? 0);
            var porcentajeCobranza = totalDeudaGeneradaMes > 0 
                ? Math.Round((transaccionesDelMes.Sum(p => p.MontoAbonado) / totalDeudaGeneradaMes) * 100, 2)
                : 0;

            // Operational Counters
            var totalPersonas = (await _personaRepository.FindAsync(p => p.ContratoParticipantes.Any(cp => cp.IdContratoNavigation.IdPropiedadNavigation.IdManzanoNavigation.IdCondominio == condominioId))).Count();
            var totalContratos = (await _contratoRepository.FindAsync(c => propIds.Contains(c.IdPropiedad) && (c.Estado == "VIGENTE" || c.Estado == null))).Count();
            var totalEgresosHistoricos = (await _egresoRepository.FindAsync(e => e.IdCondominio == condominioId)).Count();
            var totalEventos = (await _reservaRepository.FindAsync(r => r.FechaInicio >= ahora && propIds.Contains(r.IdContratoNavigation.IdPropiedad), includeProperties: "IdContratoNavigation")).Count();

            return Result.Ok(new DashboardCountersDto
            {
                TotalUnidades = totalUnidades,
                UnidadesEnMora = unidadesEnMora,
                TotalMoraHistorica = moraHistorica,
                
                TotalCobradoMesActual = totalCobradoMesActual,
                TotalEgresosMesActual = totalEgresosMesActual,
                CashFlowMesActual = totalCobradoMesActual - totalEgresosMesActual,
                
                PorcentajeCobranza = porcentajeCobranza,
                
                TotalPersonas = totalPersonas,
                TotalContratos = totalContratos,
                TotalEgresos = totalEgresosHistoricos,
                TotalEventos = totalEventos,
                
                CondominioNombre = condominio.Nombre,
                UltimaActualizacion = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            return Result.Fail<DashboardCountersDto>($"Error calculando métricas financieras: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<CategoryTotalDto>>> GetExpensesByCategoryAsync(int condominioId, int mes, int anio)
    {
        try
        {
            var primerDia = new DateTime(anio, mes, 1);
            var primerDiaSiguiente = primerDia.AddMonths(1);

            var egresos = await _egresoRepository.FindAsync(
                e => e.IdCondominio == condominioId &&
                     e.FechaEgreso >= primerDia && e.FechaEgreso < primerDiaSiguiente
            );

            var agrupados = egresos
                .GroupBy(e => e.Concepto ?? "Sin Categoría")
                .Select(g => new CategoryTotalDto
                {
                    Categoria = g.Key,
                    Total = g.Sum(e => e.MontoTotal)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            return Result.Ok<IEnumerable<CategoryTotalDto>>(agrupados);
        }
        catch (Exception ex)
        {
            return Result.Fail<IEnumerable<CategoryTotalDto>>($"Error al obtener gastos por categoría: {ex.Message}");
        }
    }
}
