using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Interfaces.Inmuebles;

public interface IManzanoService
{
    Task<Result<IEnumerable<ManzanoDto>>> GetAllAsync();
    Task<Result<IEnumerable<ManzanoDto>>> GetByCondominioAsync(int condominioId);
    Task<Result<ManzanoDto>> GetByIdAsync(int id);
    Task<Result<ManzanoDto>> CreateAsync(CreateManzanoDto dto, int condominioId);
    Task<Result<ManzanoDto>> UpdateAsync(int id, CreateManzanoDto dto, int condominioId);
    Task<Result<bool>> DeleteAsync(int id);
}
