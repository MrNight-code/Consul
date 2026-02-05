using Consulcon.Application.DTOs.Dashboard;
using Consulcon.Application.Interfaces.Dashboard;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Entities.Facturacion;

namespace Consulcon.Application.Services.Dashboard;

/// Servicio que agrega datos de múltiples repositorios para proveer una vista del estado del condominio.
public class DashboardService : IDashboardService
{
    private readonly IRepository<Propiedad> _propiedadRepository;
    private readonly IRepository<Condominio> _condominioRepository;
    private readonly IRepository<DeudaCabecera> _deudaRepository;
    private readonly IRepository<TransaccionPago> _transaccionPagoRepository;

    public DashboardService(
        IRepository<Propiedad> propiedadRepository,
        IRepository<Condominio> condominioRepository,
        IRepository<DeudaCabecera> deudaRepository,
        IRepository<TransaccionPago> transaccionPagoRepository)
    {
        _propiedadRepository = propiedadRepository;
        _condominioRepository = condominioRepository;
        _deudaRepository = deudaRepository;
        _transaccionPagoRepository = transaccionPagoRepository;
    }

    public async Task<Result<DashboardCountersDto>> ObtenerContadoresAsync(int condominioId)
    {
        return await CalcularContadoresAsync(condominioId);
    }

    public async Task<Result<DashboardCountersDto>> RefrescarContadoresAsync(int condominioId)
    {
        // El refresh fuerza un recálculo (en una versión con caché, aquí se invalidaría el caché)
        return await CalcularContadoresAsync(condominioId);
    }

    private async Task<Result<DashboardCountersDto>> CalcularContadoresAsync(int condominioId)
    {
        try
        {
            // 1. Obtener el condominio para validar existencia y obtener nombre
            var condominio = await _condominioRepository.GetByIdAsync(condominioId);
            if (condominio == null)
            {
                return Result.Fail<DashboardCountersDto>($"Condominio con ID {condominioId} no encontrado");
            }

            // 2. Total de unidades activas en el condominio
            var propiedades = await _propiedadRepository.FindAsync(
                p => p.IdManzanoNavigation.IdCondominio == condominioId && p.Activo == true,
                includeProperties: "IdManzanoNavigation"
            );
            var totalUnidades = propiedades.Count();

            // 3. Unidades en mora (saldo deudor > 0)
            var unidadesEnMora = propiedades.Count(p => p.SaldoDeudor > 0);

            // 4. Total cobrado en el mes actual
            var ahora = DateTime.Now;
            var primerDiaMes = new DateTime(ahora.Year, ahora.Month, 1);
            var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            var transaccionesDelMes = await _transaccionPagoRepository.FindAsync(
                p => p.FechaPago >= primerDiaMes && p.FechaPago <= ultimoDiaMes,
                includeProperties: "IdDeudaNavigation.IdContratoNavigation.IdPropiedadNavigation.IdManzanoNavigation"
            );

            var totalCobradoMesActual = transaccionesDelMes
                .Where(p => p.IdDeudaNavigation?.IdContratoNavigation?.IdPropiedadNavigation?.IdManzanoNavigation?.IdCondominio == condominioId)
                .Sum(p => p.MontoAbonado);

            // 5. Calcular porcentaje de cobranza
            var deudasDelMes = await _deudaRepository.FindAsync(
                d => d.AnioPeriodo == ahora.Year &&
                     d.MesPeriodo == ahora.Month,
                includeProperties: "IdContratoNavigation.IdPropiedadNavigation.IdManzanoNavigation"
            );

            var totalDeudaMesActual = deudasDelMes
                .Where(d => d.IdContratoNavigation?.IdPropiedadNavigation?.IdManzanoNavigation?.IdCondominio == condominioId)
                .Sum(d => d.TotalDeuda ?? 0);

            var porcentajeCobranza = totalDeudaMesActual > 0 
                ? Math.Round((totalCobradoMesActual / totalDeudaMesActual) * 100, 2)
                : 0;

            // 6. Construir DTO
            var dto = new DashboardCountersDto
            {
                TotalUnidades = totalUnidades,
                UnidadesEnMora = unidadesEnMora,
                TotalCobradoMesActual = totalCobradoMesActual,
                PorcentajeCobranza = porcentajeCobranza,
                CondominioNombre = condominio.Nombre,
                UltimaActualizacion = DateTime.Now
            };

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result.Fail<DashboardCountersDto>($"Error al calcular contadores del dashboard: {ex.Message}");
        }
    }
}
