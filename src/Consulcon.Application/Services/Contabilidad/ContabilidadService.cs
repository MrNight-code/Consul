using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;

namespace Consulcon.Application.Services.Contabilidad;

public class ContabilidadService : IContabilidadService
{
    private readonly IRepository<PlanCuenta> _planCuentaRepository;
    private readonly IRepository<AsientoContable> _asientoRepository;
    private readonly IRepository<AsientoDetalle> _detalleRepository;
    private readonly IRepository<AutorizacionGasto> _autorizacionRepository;

    public ContabilidadService(
        IRepository<PlanCuenta> planCuentaRepository,
        IRepository<AsientoContable> asientoRepository,
        IRepository<AsientoDetalle> detalleRepository,
        IRepository<AutorizacionGasto> autorizacionRepository)
    {
        _planCuentaRepository = planCuentaRepository;
        _asientoRepository = asientoRepository;
        _detalleRepository = detalleRepository;
        _autorizacionRepository = autorizacionRepository;
    }

    public async Task<Result<IEnumerable<PlanCuentaDto>>> GetPlanCuentasAsync()
    {
        var entities = await _planCuentaRepository.GetAllAsync();
        var dtos = entities.Select(e => new PlanCuentaDto
        {
            Id = e.IdCuenta,
            CodigoCuenta = e.CodigoCuenta,
            Nombre = e.Nombre,
            IdCuentaPadre = e.IdCuentaPadre,
            NivelJerarquia = e.NivelJerarquia,
            EsImputable = e.EsImputable
        });
        return Result.Ok(dtos);
    }

    public async Task<Result<PlanCuentaDto>> CreateCuentaAsync(PlanCuentaDto dto)
    {
        var entity = new PlanCuenta
        {
            CodigoCuenta = dto.CodigoCuenta,
            Nombre = dto.Nombre,
            IdCuentaPadre = dto.IdCuentaPadre,
            NivelJerarquia = dto.NivelJerarquia ?? 1,
            EsImputable = dto.EsImputable ?? true
        };
        await _planCuentaRepository.AddAsync(entity);
        dto.Id = entity.IdCuenta;
        return Result.Ok(dto);
    }

    public async Task<Result<IEnumerable<AsientoDto>>> GetAsientosByCondominioAsync(int condominioId)
    {
        var entities = await _asientoRepository.FindAsync(a => a.IdCondominio == condominioId, 
            includeProperties: "AsientoDetalles,AsientoDetalles.IdCuentaNavigation");
        
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<AsientoDto>> RegistrarAsientoAsync(CreateAsientoDto dto)
    {
        if (dto.Detalles.Sum(d => d.Debe) != dto.Detalles.Sum(d => d.Haber))
        {
            if (Math.Abs(dto.Detalles.Sum(d => d.Debe) - dto.Detalles.Sum(d => d.Haber)) > 0.01m)
                    return Result.Fail<AsientoDto>("El asiento no balancea (Debe != Haber)");
        }

        var asiento = new AsientoContable
        {
            IdCondominio = dto.IdCondominio,
            FechaContable = dto.FechaContable,
            GlosaGeneral = dto.GlosaGeneral,
            TipoAsiento = dto.TipoAsiento,
            NroDocumentoRespaldo = dto.NroDocumentoRespaldo
        };

        await _asientoRepository.AddAsync(asiento);

        foreach (var det in dto.Detalles)
        {
            var detalle = new AsientoDetalle
            {
                IdAsiento = asiento.IdAsiento,
                IdCuenta = det.IdCuenta,
                GlosaLinea = det.GlosaLinea,
                Debe = det.Debe,
                Haber = det.Haber
            };
            await _detalleRepository.AddAsync(detalle);
        }

        return await GetAsientoById(asiento.IdAsiento);
    }

    public async Task<Result<IEnumerable<AutorizacionGastoDto>>> GetAutorizacionesAsync()
    {
        var entities = await _autorizacionRepository.GetAllAsync();
        var dtos = entities.Select(e => new AutorizacionGastoDto
        {
            IdAutorizacion = e.IdAutorizacion,
            Descripcion = e.Descripcion,
            Activo = e.Activo
        });
        return Result.Ok(dtos);
    }

    public async Task<Result<AutorizacionGastoDto>> CreateAutorizacionAsync(AutorizacionGastoDto dto)
    {
        var entity = new AutorizacionGasto
        {
            Descripcion = dto.Descripcion,
            Activo = dto.Activo ?? true
        };
        await _autorizacionRepository.AddAsync(entity);
        dto.IdAutorizacion = entity.IdAutorizacion;
        return Result.Ok(dto);
    }

    private async Task<Result<AsientoDto>> GetAsientoById(int id)
    {
            var entities = await _asientoRepository.FindAsync(a => a.IdAsiento == id, 
            includeProperties: "AsientoDetalles,AsientoDetalles.IdCuentaNavigation");
            var entity = entities.FirstOrDefault();
            if (entity == null) return Result.Fail<AsientoDto>("Asiento no encontrado");
            return Result.Ok(MapToDto(entity));
    }

    private static AsientoDto MapToDto(AsientoContable entity)
    {
        return new AsientoDto
        {
            Id = entity.IdAsiento,
            IdCondominio = entity.IdCondominio,
            FechaContable = entity.FechaContable,
            GlosaGeneral = entity.GlosaGeneral,
            TipoAsiento = entity.TipoAsiento,
            NroDocumentoRespaldo = entity.NroDocumentoRespaldo,
            Detalles = entity.AsientoDetalles.Select(d => new AsientoDetalleDto
            {
                Id = d.IdAsientoDet,
                IdCuenta = d.IdCuenta,
                CuentaNombre = d.IdCuentaNavigation?.Nombre,
                GlosaLinea = d.GlosaLinea,
                Debe = d.Debe,
                Haber = d.Haber
            }).ToList()
        };
    }
}

