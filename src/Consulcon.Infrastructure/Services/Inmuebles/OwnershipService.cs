using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Consulcon.Domain.Common;
using Consulcon.Domain.Constants;
using Consulcon.Domain.Entities.Contratos;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Consulcon.Infrastructure.Services.Inmuebles;

/// <summary>
/// Service for managing property ownership assignments and history.
/// </summary>
public class OwnershipService(ConsulconDbContext context) : IOwnershipService
{
    private readonly ConsulconDbContext _context = context;

    /// <inheritdoc/>
    public async Task<Result<OwnershipHistoryDto>> AssignOwnerAsync(AssignOwnerDto dto)
    {
        // Validation: Check property exists
        var propiedad = await _context.Propiedads
            .Include(p => p.Contratos)
                .ThenInclude(c => c.ContratoParticipantes)
                    .ThenInclude(cp => cp.IdPersonaNavigation)
            .FirstOrDefaultAsync(p => p.IdPropiedad == dto.PropiedadId);

        if (propiedad == null)
            return Result.Fail<OwnershipHistoryDto>($"La propiedad con ID {dto.PropiedadId} no existe.");

        // Validation: Check new owner (Persona) exists
        var nuevoDueno = await _context.Personas.FindAsync(dto.NuevoDuenoId);
        if (nuevoDueno == null)
            return Result.Fail<OwnershipHistoryDto>($"La persona con ID {dto.NuevoDuenoId} no existe.");

        // Start transaction for atomic operation
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Find active contract for the property
            var activeContract = propiedad.Contratos
                .OrderByDescending(c => c.FechaInicio)
                .FirstOrDefault(c => c.Estado == OwnershipConstants.EstadoVigente || c.Estado == null);

            // If no active contract, create one
            if (activeContract == null)
            {
                activeContract = new Contrato
                {
                    IdPropiedad = dto.PropiedadId,
                    FechaInicio = dto.FechaInicio,
                    Estado = OwnershipConstants.EstadoVigente,
                    MontoExpensaPactada = propiedad.ExpensaBaseDefecto ?? 0
                };
                _context.Contratos.Add(activeContract);
                await _context.SaveChangesAsync();
            }

            // Find current active "Titular" (owner) in the contract
            var currentOwner = activeContract.ContratoParticipantes
                .FirstOrDefault(cp => cp.RolContrato == OwnershipConstants.RolTitular && cp.FechaBaja == null);

            // Validation: Check for date overlaps
            if (currentOwner != null && currentOwner.FechaAlta.HasValue && dto.FechaInicio < currentOwner.FechaAlta.Value)
            {
                return Result.Fail<OwnershipHistoryDto>(
                    $"La fecha de inicio ({dto.FechaInicio:yyyy-MM-dd}) no puede ser anterior a la fecha de alta del dueño actual ({currentOwner.FechaAlta.Value:yyyy-MM-dd}).");
            }

            // Close the existing ownership record (set FechaBaja)
            if (currentOwner != null)
            {
                // Set end date to the day before new ownership starts (no overlap)
                currentOwner.FechaBaja = dto.FechaInicio.AddDays(-1);
                currentOwner.Activo = false;
            }

            // Create new ownership record
            var newOwnership = new ContratoParticipante
            {
                IdContrato = activeContract.IdContrato,
                IdPersona = dto.NuevoDuenoId,
                RolContrato = OwnershipConstants.RolTitular,
                FechaAlta = dto.FechaInicio,
                FechaBaja = null,
                Activo = true
            };
            _context.ContratoParticipantes.Add(newOwnership);

            // Save and commit transaction
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            // Build response DTO
            var result = new OwnershipHistoryDto
            {
                ContratoId = activeContract.IdContrato,
                PersonaId = dto.NuevoDuenoId,
                NombrePersona = nuevoDueno.NombreCompleto,
                FechaInicio = dto.FechaInicio,
                FechaFin = null,
                EsVigente = true
            };

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result.Fail<OwnershipHistoryDto>($"Error al asignar propietario: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<OwnershipHistoryDto>>> GetOwnershipHistoryAsync(int propiedadId)
    {
        // Check property exists
        var propiedadExists = await _context.Propiedads.AnyAsync(p => p.IdPropiedad == propiedadId);
        if (!propiedadExists)
            return Result.Fail<List<OwnershipHistoryDto>>($"La propiedad con ID {propiedadId} no existe.");

        // Get all contracts for the property
        var contratos = await _context.Contratos
            .Include(c => c.ContratoParticipantes)
                .ThenInclude(cp => cp.IdPersonaNavigation)
            .Where(c => c.IdPropiedad == propiedadId)
            .ToListAsync();

        // Get all "Titular" participants across all contracts, ordered chronologically
        var history = contratos
            .SelectMany(c => c.ContratoParticipantes
                .Where(cp => cp.RolContrato == OwnershipConstants.RolTitular)
                .Select(cp => new OwnershipHistoryDto
                {
                    ContratoId = c.IdContrato,
                    PersonaId = cp.IdPersona,
                    NombrePersona = cp.IdPersonaNavigation?.NombreCompleto ?? "Desconocido",
                    FechaInicio = cp.FechaAlta ?? c.FechaInicio,
                    FechaFin = cp.FechaBaja,
                    EsVigente = cp.FechaBaja == null && cp.Activo == true
                }))
            .OrderByDescending(h => h.FechaInicio)
            .ToList();

        return Result.Ok(history);
    }
}
