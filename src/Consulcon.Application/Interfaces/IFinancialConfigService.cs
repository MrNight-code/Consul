using System.Collections.Generic;
using System.Threading.Tasks;
using Consulcon.Application.DTOs.Financiero;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Interfaces;

public interface IFinancialConfigService
{
    // Charge Concepts
    Task<Result<IEnumerable<ChargeConceptDto>>> GetChargeConceptsAsync(int condominiumId);
    Task<Result<int>> CreateChargeConceptAsync(int condominiumId, CreateChargeConceptDto dto);
    Task<Result<bool>> UpdateChargeConceptAsync(int id, UpdateChargeConceptDto dto);
    Task<Result<bool>> DeleteChargeConceptAsync(int id);

    // Financial Config (Penalties)
    Task<Result<FinancialConfigDto>> GetFinancialConfigAsync(int condominiumId);
    Task<Result<bool>> UpdateFinancialConfigAsync(int condominiumId, UpdateFinancialConfigDto dto);
}
