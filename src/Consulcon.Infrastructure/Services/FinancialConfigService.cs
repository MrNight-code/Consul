using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consulcon.Application.DTOs.Financiero;
using Consulcon.Application.Interfaces;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Financiero;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Consulcon.Infrastructure.Services;

public class FinancialConfigService(ConsulconDbContext context) : IFinancialConfigService
{
    private readonly ConsulconDbContext _context = context;

    public async Task<Result<IEnumerable<ChargeConceptDto>>> GetChargeConceptsAsync(int condominiumId)
    {
        var entities = await _context.ChargeConcepts
            .Where(x => x.CondominiumId == condominiumId && x.IsActive)
            .ToListAsync();

        var dtos = entities.Select(e => new ChargeConceptDto
        {
            Id = e.Id,
            Name = e.Name,
            Code = e.Code,
            IsRecurrent = e.IsRecurrent,
            IsActive = e.IsActive
        });

        return Result.Ok<IEnumerable<ChargeConceptDto>>(dtos);
    }

    public async Task<Result<int>> CreateChargeConceptAsync(int condominiumId, CreateChargeConceptDto dto)
    {
        var entity = new ChargeConcept
        {
            CondominiumId = condominiumId,
            Name = dto.Name,
            Code = dto.Code,
            IsRecurrent = dto.IsRecurrent,
            IsActive = true
        };

        _context.ChargeConcepts.Add(entity);
        await _context.SaveChangesAsync();

        return Result.Ok(entity.Id);
    }

    public async Task<Result<bool>> UpdateChargeConceptAsync(int id, UpdateChargeConceptDto dto)
    {
        var entity = await _context.ChargeConcepts.FindAsync(id);
        if (entity == null)
            return Result.Fail<bool>("Concepto no encontrado.");

        entity.Name = dto.Name;
        entity.Code = dto.Code;
        entity.IsRecurrent = dto.IsRecurrent;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return Result.Ok(true);
    }

    public async Task<Result<bool>> DeleteChargeConceptAsync(int id)
    {
        var entity = await _context.ChargeConcepts.FindAsync(id);
        if (entity == null)
            return Result.Fail<bool>("Concepto no encontrado.");

        // Soft delete
        entity.IsActive = false;
        await _context.SaveChangesAsync();
        return Result.Ok(true);
    }

    public async Task<Result<FinancialConfigDto>> GetFinancialConfigAsync(int condominiumId)
    {
        var entity = await _context.FinancialConfigs
            .FirstOrDefaultAsync(x => x.CondominiumId == condominiumId);

        if (entity == null)
        {
            // Return default if not exists
            return Result.Ok(new FinancialConfigDto
            {
                CondominiumId = condominiumId,
                MonthlyInterestRate = 0,
                GraceDays = 0
            });
        }

        return Result.Ok(new FinancialConfigDto
        {
            Id = entity.Id,
            CondominiumId = entity.CondominiumId,
            MonthlyInterestRate = entity.MonthlyInterestRate,
            GraceDays = entity.GraceDays
        });
    }

    public async Task<Result<bool>> UpdateFinancialConfigAsync(int condominiumId, UpdateFinancialConfigDto dto)
    {
        var entity = await _context.FinancialConfigs
            .FirstOrDefaultAsync(x => x.CondominiumId == condominiumId);

        if (entity == null)
        {
            entity = new FinancialConfig
            {
                CondominiumId = condominiumId,
                MonthlyInterestRate = dto.MonthlyInterestRate,
                GraceDays = dto.GraceDays
            };
            _context.FinancialConfigs.Add(entity);
        }
        else
        {
            entity.MonthlyInterestRate = dto.MonthlyInterestRate;
            entity.GraceDays = dto.GraceDays;
        }

        await _context.SaveChangesAsync();
        return Result.Ok(true);
    }
}
