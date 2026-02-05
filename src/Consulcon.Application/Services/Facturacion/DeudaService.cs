using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Application.Interfaces.Facturacion;

namespace Consulcon.Application.Services.Facturacion;

public class DeudaService : IDeudaService
{
    private readonly IRepository<DeudaCabecera> _deudaRepository;
    private readonly IRepository<DeudaDetalle> _detalleRepository;

    public DeudaService(IRepository<DeudaCabecera> deudaRepository, IRepository<DeudaDetalle> detalleRepository)
    {
        _deudaRepository = deudaRepository;
        _detalleRepository = detalleRepository;
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
