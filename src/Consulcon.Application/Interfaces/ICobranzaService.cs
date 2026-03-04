using Consulcon.Application.DTOs;
using Consulcon.Domain.Common;
using System.Threading.Tasks;

namespace Consulcon.Application.Interfaces
{
    public interface ICobranzaService
    {
        Task<Result<bool>> RegistrarCobranzaAsync(CobranzaRequest request);
        Task<Result<List<CobranzaDto>>> ObtenerHistorialAsync(int unitId);
        Task<Result<PagedResult<CobranzaDto>>> GetPagedAsync(int idCondominio, PaginationParams parameters);
    }
}
