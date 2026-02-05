using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;

namespace Consulcon.Application.Services.Contabilidad;

public class TesoreriaService : ITesoreriaService
{
    private readonly IRepository<Banco> _bancoRepository;
    private readonly IRepository<FormaPago> _fpRepository;
    private readonly IRepository<Egreso> _egresoRepository;

    public TesoreriaService(
        IRepository<Banco> bancoRepository, 
        IRepository<FormaPago> fpRepository,
        IRepository<Egreso> egresoRepository)
    {
        _bancoRepository = bancoRepository;
        _fpRepository = fpRepository;
        _egresoRepository = egresoRepository;
    }

    public async Task<Result<IEnumerable<BancoDto>>> GetBancosAsync()
    {
        var entities = await _bancoRepository.GetAllAsync();
        return Result.Ok(entities.Select(e => new BancoDto { Id = e.IdBanco, NombreEntidad = e.NombreEntidad, Moneda = e.Moneda, NumeroCuenta = e.NumeroCuenta, Activo = e.Activo }));
    }

    public async Task<Result<IEnumerable<FormaPagoDto>>> GetFormasPagoAsync()
    {
        var entities = await _fpRepository.GetAllAsync();
        return Result.Ok(entities.Select(e => new FormaPagoDto { Id = e.IdFormaPago, Descripcion = e.Descripcion }));
    }

    public async Task<Result<BancoDto>> CreateBancoAsync(BancoDto dto)
    {
        var entity = new Banco { NombreEntidad = dto.NombreEntidad, NumeroCuenta = dto.NumeroCuenta, Moneda = dto.Moneda, Activo = true };
        await _bancoRepository.AddAsync(entity);
        dto.Id = entity.IdBanco;
        return Result.Ok(dto);
    }

    public async Task<Result<FormaPagoDto>> CreateFormaPagoAsync(FormaPagoDto dto)
    {
        var entity = new FormaPago { Descripcion = dto.Descripcion };
        await _fpRepository.AddAsync(entity);
        dto.Id = entity.IdFormaPago;
        return Result.Ok(dto);
    }

    public async Task<Result<IEnumerable<EgresoDto>>> GetEgresosByCondominioAsync(int condominioId)
    {
        var entities = await _egresoRepository.FindAsync(e => e.IdCondominio == condominioId, 
            includeProperties: "IdProveedorNavigation,IdPersonaBeneficiarioNavigation,IdBancoOrigenNavigation");
        
        return Result.Ok(entities.Select(MapEgresoToDto));
    }

    public async Task<Result<EgresoDto>> RegistrarEgresoAsync(CreateEgresoDto dto)
    {
        var entity = new Egreso
        {
            IdCondominio = dto.IdCondominio,
            IdProveedor = dto.IdProveedor,
            IdPersonaBeneficiario = dto.IdPersonaBeneficiario,
            IdAutorizacion = dto.IdAutorizacion,
            IdBancoOrigen = dto.IdBancoOrigen,
            IdFormaPago = dto.IdFormaPago,
            Concepto = dto.Concepto,
            MontoTotal = dto.MontoTotal,
            FechaEgreso = DateTime.Now,
            NroFacturaProveedor = dto.NroFacturaProveedor,
            IdUsuarioRegistro = dto.IdUsuarioRegistro
        };

        await _egresoRepository.AddAsync(entity);
        
        // Re-fetch or simple mapping (needs navigation properties ideally)
        // For speed, just mapping basic success
            
        // var created = await _egresoRepository.GetByIdAsync(entity.IdEgreso); // Lazy way to get navigations if configured, or just basic
        
        return Result.Ok(MapEgresoToDto(entity));
    }

    private static EgresoDto MapEgresoToDto(Egreso e)
    {
        return new EgresoDto
        {
            Id = e.IdEgreso,
            IdCondominio = e.IdCondominio,
            IdProveedor = e.IdProveedor,
            ProveedorNombre = e.IdProveedorNavigation?.RazonSocial,
            IdPersonaBeneficiario = e.IdPersonaBeneficiario,
            BeneficiarioNombre = e.IdPersonaBeneficiarioNavigation?.NombreCompleto,
            IdAutorizacion = e.IdAutorizacion,
            IdBancoOrigen = e.IdBancoOrigen,
            BancoNombre = e.IdBancoOrigenNavigation?.NombreEntidad,
            IdFormaPago = e.IdFormaPago,
            Concepto = e.Concepto,
            MontoTotal = e.MontoTotal,
            FechaEgreso = e.FechaEgreso,
            NroFacturaProveedor = e.NroFacturaProveedor
        };
    }
}
