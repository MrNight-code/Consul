using Consulcon.Application.DTOs.Inmuebles;

namespace Consulcon.Application.Interfaces.Inmuebles;

public interface ICondominioService
{
    Task<Result<IEnumerable<CondominioDto>>> GetAllAsync(int userId);
    Task<Result<CondominioDto>> GetByIdAsync(int id);
    Task<Result<CondominioDto>> CreateAsync(CondominioDto dto, int userId);
    Task<Result<CondominioDto>> UpdateAsync(int id, CondominioDto dto);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<bool>> AddUserAsync(int condominioId, AddUserToCondominioDto dto);
    Task<Result<IEnumerable<CondominioUserDto>>> GetUsersAsync(int condominioId);
    Task<Result<bool>> RemoveUserAsync(int condominioId, int userId);
}
