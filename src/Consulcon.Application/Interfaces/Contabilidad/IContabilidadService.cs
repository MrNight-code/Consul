using Consulcon.Application.DTOs.Contabilidad;

namespace Consulcon.Application.Interfaces.Contabilidad;

public interface IContabilidadService
{
    Task<Result<IEnumerable<PlanCuentaDto>>> GetPlanCuentasAsync();
    Task<Result<PlanCuentaDto>> CreateCuentaAsync(PlanCuentaDto dto);
    
    Task<Result<IEnumerable<AsientoDto>>> GetAsientosByCondominioAsync(int condominioId);
    Task<Result<AsientoDto>> RegistrarAsientoAsync(CreateAsientoDto dto);
    
    Task<Result<IEnumerable<AutorizacionGastoDto>>> GetAutorizacionesAsync();
    Task<Result<AutorizacionGastoDto>> CreateAutorizacionAsync(AutorizacionGastoDto dto);
}

