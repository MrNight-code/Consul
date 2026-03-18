using Consulcon.Application.DTOs.Expensas;
using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;
using Consulcon.Domain.Entities.Inmuebles;

namespace Consulcon.Application.Services.Facturacion;

public class DeudaService : IDeudaService
{
    private readonly IRepository<DeudaCabecera> _deudaRepository;
    private readonly IRepository<DeudaDetalle> _detalleRepository;
    private readonly IRepository<Propiedad> _propiedadRepository;

    public DeudaService(
        IRepository<DeudaCabecera> deudaRepository, 
        IRepository<DeudaDetalle> detalleRepository,
        IRepository<Propiedad> propiedadRepository)
    {
        _deudaRepository = deudaRepository;
        _detalleRepository = detalleRepository;
        _propiedadRepository = propiedadRepository;
    }

    public async Task<Result<IEnumerable<DeudaDto>>> GetByContratoAsync(int contratoId)
    {
        var entities = await _deudaRepository.FindAsync(d => d.IdContrato == contratoId, 
            includeProperties: "IdContratoNavigation,IdContratoNavigation.IdPropiedadNavigation,DeudaDetalles,DeudaDetalles.IdServicioNavigation");
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<DeudaDto>>> GetPendingAsync()
    {
        var entities = await _deudaRepository.FindAsync(d => d.EstadoPago != "PAGADO" && d.EstadoPago != "ANULADO",
            includeProperties: "IdContratoNavigation,IdContratoNavigation.IdPropiedadNavigation,DeudaDetalles,DeudaDetalles.IdServicioNavigation");
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<DeudaDto>> GenerateDeudaAsync(GenerateDeudaDto dto)
    {
        var deuda = new DeudaCabecera
        {
            IdContrato = dto.IdContrato,
            AnioPeriodo = dto.Anio,
            MesPeriodo = dto.Mes,
            FechaEmision = DateOnly.FromDateTime(DateTime.Now),
            FechaVencimiento = dto.FechaVencimiento,
            TotalDeuda = 0,
            TotalPagado = 0,
            EstadoPago = "PENDIENTE",
            IdUsuarioGenerador = dto.IdUsuarioGenerador
        };

        await _deudaRepository.AddAsync(deuda);

        decimal total = 0;
        if (dto.DetallesAdicionales != null)
        {
            foreach (var det in dto.DetallesAdicionales)
            {
                var subtotal = det.MontoUnitario * det.Cantidad;
                var detalle = new DeudaDetalle
                {
                    IdDeuda = deuda.IdDeuda,
                    IdServicio = det.IdServicio,
                    Concepto = det.Concepto,
                    MontoUnitario = det.MontoUnitario,
                    Cantidad = det.Cantidad,
                    Subtotal = subtotal
                };
                await _detalleRepository.AddAsync(detalle);
                total += subtotal;
            }
        }

        deuda.TotalDeuda = total;
        await _deudaRepository.UpdateAsync(deuda);

        return await GetByIdAsync(deuda.IdDeuda);
    }

    public async Task<Result<DeudaDto>> GetByIdAsync(int id)
    {
        var entities = await _deudaRepository.FindAsync(d => d.IdDeuda == id,
            includeProperties: "IdContratoNavigation,IdContratoNavigation.IdPropiedadNavigation,DeudaDetalles,DeudaDetalles.IdServicioNavigation");
        var entity = entities.FirstOrDefault();
        if (entity == null) return Result.Fail<DeudaDto>("Deuda no encontrada");
        
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<EstadoCuentaUnidadResponseDto>> GetEstadoCuentaByPropiedadAsync(int propiedadId)
    {
        var propiedades = await _propiedadRepository.FindAsync(
            p => p.IdPropiedad == propiedadId,
            includeProperties: "Contratos,Contratos.DeudaCabeceras,Contratos.DeudaCabeceras.DeudaDetalles,Contratos.DeudaCabeceras.DeudaDetalles.IdServicioNavigation,Contratos.ContratoParticipantes,Contratos.ContratoParticipantes.IdPersonaNavigation"
        );

        var propiedad = propiedades.FirstOrDefault();
        if (propiedad == null) return Result.Fail<EstadoCuentaUnidadResponseDto>("Propiedad no encontrada");

        var contratoVigente = propiedad.Contratos.FirstOrDefault(c => c.Estado == "Vigente");
        
        var propietarioActual = contratoVigente?.ContratoParticipantes
            .FirstOrDefault(cp => cp.RolContrato == "T" && cp.Activo == true && cp.FechaBaja == null);
            
        string nombrePropietario = propietarioActual?.IdPersonaNavigation?.NombreCompleto ?? string.Empty;

        var dto = new EstadoCuentaUnidadResponseDto
        {
            FkPropiedad = propiedad.IdPropiedad,
            CodigoUnidad = propiedad.CodigoUnidad,
            NombreUnidad = propiedad.NombreFuncional ?? $"Unidad {propiedad.IdPropiedad}",
            Propietario = nombrePropietario,
            SaldoVencido = 0,
            SaldoVigente = 0,
            SaldoTotal = 0,
            SaldoAFavor = propiedad.SaldoAFavor,
            Conceptos = new List<ConceptoDeudaResponseDto>()
        };

        if (contratoVigente != null && contratoVigente.DeudaCabeceras != null)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            foreach (var deuda in contratoVigente.DeudaCabeceras.Where(d => d.EstadoPago != "ANULADO").OrderByDescending(d => d.FechaVencimiento))
            {
                decimal totalDeuda = deuda.TotalDeuda ?? 0m;
                decimal totalPagado = deuda.TotalPagado ?? 0m;
                decimal saldoPendienteDeuda = totalDeuda - totalPagado;
                
                if (deuda.EstadoPago == "PENDIENTE" || deuda.EstadoPago == "PARCIAL")
                {
                    dto.SaldoTotal += saldoPendienteDeuda;
                    
                    if (deuda.FechaVencimiento < hoy)
                        dto.SaldoVencido += saldoPendienteDeuda;
                    else
                        dto.SaldoVigente += saldoPendienteDeuda;
                }

                var conceptoStr = deuda.DeudaDetalles != null && deuda.DeudaDetalles.Any() 
                    ? string.Join(", ", deuda.DeudaDetalles.Select(d => d.IdServicioNavigation?.Nombre ?? d.Concepto))
                    : "Deuda";

                var tipoConceptoStr = deuda.DeudaDetalles?.FirstOrDefault()?.IdServicioNavigation?.Nombre ?? string.Empty;

                // El monto a mostrar en la tabla será el saldo pendiente del ticket si no está pagado, o el total si está pagado.
                decimal montoMostrar = (deuda.EstadoPago == "PENDIENTE" || deuda.EstadoPago == "PARCIAL") && saldoPendienteDeuda > 0 
                    ? saldoPendienteDeuda 
                    : totalDeuda;

                var conceptoDto = new ConceptoDeudaResponseDto
                {
                    PkDeuda = deuda.IdDeuda,
                    Concepto = conceptoStr,
                    Mes = deuda.MesPeriodo,
                    Ano = deuda.AnioPeriodo,
                    Monto = montoMostrar,
                    FechaVencimiento = deuda.FechaVencimiento,
                    Estado = deuda.EstadoPago ?? string.Empty,
                    TipoConcepto = tipoConceptoStr
                };
                dto.Conceptos.Add(conceptoDto);
            }
        }

        return Result.Ok(dto);
    }

    private static DeudaDto MapToDto(DeudaCabecera entity)
    {
        return new DeudaDto
        {
            Id = entity.IdDeuda,
            IdContrato = entity.IdContrato,
            ContratoInfo = entity.IdContratoNavigation?.IdPropiedadNavigation != null 
                ? $"{entity.IdContratoNavigation.IdPropiedadNavigation.CodigoUnidad} - {entity.IdContratoNavigation.IdPropiedadNavigation.NombreFuncional}" 
                : $"Contrato {entity.IdContrato}",
            AnioPeriodo = entity.AnioPeriodo,
            MesPeriodo = entity.MesPeriodo,
            FechaEmision = entity.FechaEmision,
            FechaVencimiento = entity.FechaVencimiento,
            TotalDeuda = entity.TotalDeuda,
            TotalPagado = entity.TotalPagado,
            EstadoPago = entity.EstadoPago,
            Detalles = entity.DeudaDetalles.Select(d => new DeudaDetalleDto
            {
                Id = d.IdDeudaDet,
                IdServicio = d.IdServicio,
                ServicioNombre = d.IdServicioNavigation?.Nombre,
                Concepto = d.Concepto,
                MontoUnitario = d.MontoUnitario,
                Cantidad = d.Cantidad,
                Subtotal = d.Subtotal
            }).ToList()
        };
    }
}
