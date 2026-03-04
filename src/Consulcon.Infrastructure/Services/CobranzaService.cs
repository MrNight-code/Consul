using Consulcon.Application.DTOs;
using Consulcon.Application.Interfaces;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Specifications;
using Consulcon.Infrastructure.Persistence;
using Consulcon.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consulcon.Infrastructure.Services
{
    public class CobranzaService(ConsulconDbContext context, IRepository<TransaccionPago> repository) : ICobranzaService
    {
        private readonly ConsulconDbContext _context = context;

        public async Task<Result<List<CobranzaDto>>> ObtenerHistorialAsync(int unitId)
        {
            var historial = await _context.TransaccionPagos
                .Include(t => t.IdDeudaNavigation)
                    .ThenInclude(d => d.IdContratoNavigation)
                .Include(t => t.IdFormaPagoNavigation)
                .Where(t => t.IdDeudaNavigation.IdContratoNavigation.IdPropiedad == unitId)
                .OrderByDescending(t => t.FechaPago)
                .Select(t => new CobranzaDto
                {
                    IdPago = t.IdPago,
                    Monto = t.MontoAbonado,
                    Fecha = t.FechaPago ?? DateTime.MinValue,
                    MetodoPago = t.IdFormaPagoNavigation.Descripcion,
                    Referencia = t.NroComprobanteBanco,
                    Observaciones = t.Observaciones,
                    Estado = t.Estado,
                    ConceptoDeuda = $"{t.IdDeudaNavigation.MesPeriodo}/{t.IdDeudaNavigation.AnioPeriodo}" 
                })
                .ToListAsync();

            return Result.Ok(historial);
        }

        public async Task<Result<bool>> RegistrarCobranzaAsync(CobranzaRequest request)
        {
            // 1. Validations
            if (request.Monto <= 0)
                return Result.Fail<bool>("El monto debe ser mayor a cero.");

            if (!request.IdBancoDestino.HasValue)
                return Result.Fail<bool>("Debe especificar la cuenta/banco de destino.");

            // 2. Transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 3. Get Unit 
                var propiedad = await _context.Propiedads
                    .Include(p => p.Contratos)
                        .ThenInclude(c => c.ContratoParticipantes)
                    .FirstOrDefaultAsync(p => p.IdPropiedad == request.UnitId);

                if (propiedad == null)
                    return Result.Fail<bool>($"La unidad con ID {request.UnitId} no existe.");

                // Resolve Payer (Persona)
                // Priority: Active Contract -> 'Titular' role -> First Participant
                // Fallback: If no contract/participant, fail? OR require it in request?
                // For MVP/User Story: "Vinculada a HousingUnit". We try to find the payer.
                var activeContract = propiedad.Contratos
                    .OrderByDescending(c => c.FechaInicio)
                    .FirstOrDefault(c => c.Estado == "VIGENTE" || c.Estado == null);

                activeContract ??= propiedad.Contratos.OrderByDescending(c => c.FechaInicio).FirstOrDefault();

                int? idPersonaPagador = null;
                if (activeContract != null)
                {
                    var titular = activeContract.ContratoParticipantes
                        .FirstOrDefault(cp => cp.RolContrato == "Titular") 
                        ?? activeContract.ContratoParticipantes.FirstOrDefault();
                    
                    if (titular != null) idPersonaPagador = titular.IdPersona;
                }

                if (idPersonaPagador == null)
                    return Result.Fail<bool>("No se encontró un titular/pagador asociado a la unidad para registrar el cobro.");

                // Validate Banco Exists
                var bancoExists = await _context.Bancos.AnyAsync(b => b.IdBanco == request.IdBancoDestino.Value && b.Activo == true);
                if (!bancoExists)
                    return Result.Fail<bool>("La cuenta de destino especificada no existe o no está activa.");

                // 4. Update Transactional Balance
                propiedad.SaldoDeudor -= request.Monto;
                if (propiedad.SaldoDeudor < 0) propiedad.SaldoDeudor = 0; 

                // 5. FIFO Logic
                var activeDebts = await _context.DeudaCabeceras
                    .Include(d => d.IdContratoNavigation)
                    .Where(d => d.IdContratoNavigation.IdPropiedad == request.UnitId 
                             && d.EstadoPago != "PAGADO")
                    .OrderBy(d => d.FechaVencimiento) 
                    .ToListAsync();

                decimal remainingAmount = request.Monto;

                foreach (var deuda in activeDebts)
                {
                    if (remainingAmount <= 0) break;

                    decimal deudaTotal = deuda.TotalDeuda ?? 0;
                    decimal pagadoPrevio = deuda.TotalPagado ?? 0;
                    decimal pendiente = deudaTotal - pagadoPrevio;

                    decimal aPagar = Math.Min(remainingAmount, pendiente);

                    // Update Debt Header
                    deuda.TotalPagado = pagadoPrevio + aPagar;
                    if (deuda.TotalPagado >= deudaTotal)
                        deuda.EstadoPago = "PAGADO";
                    else
                        deuda.EstadoPago = "PARCIAL";

                    // Create TransaccionPago
                    var pago = new TransaccionPago
                    {
                        IdPago = 0, 
                        IdDeuda = deuda.IdDeuda,
                        IdPersonaPagador = idPersonaPagador.Value, 
                        IdBancoDestino = request.IdBancoDestino.Value,
                        IdFormaPago = request.IdFormaPago,
                        FechaPago = DateTime.UtcNow, 
                        MontoAbonado = aPagar,
                        NroComprobanteBanco = request.NroReferencia,
                        Observaciones = request.Observaciones,
                        Estado = "CONFIRMADO",
                        TipoCambio = 1
                    };
                    _context.TransaccionPagos.Add(pago);

                    remainingAmount -= aPagar;
                }
                
                // Save Changes
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Result.Ok(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Fail<bool>($"Error al registrar cobranza: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<CobranzaDto>>> GetPagedAsync(int idCondominio, PaginationParams p)
        {
            // 1. Instanciar la especificación con la ruta compleja que armamos
            var spec = new CobranzaWithFiltersSpec(p, idCondominio);

            // 2. Ejecutar la consulta en la base de datos (repository debe ser IRepository<TransaccionPago>)
            var pagedData = await repository.GetPagedAsync(spec, p.PageNumber, p.PageSize);
            // 3. Mapear la entidad al DTO
            var result = pagedData.Map(x => new CobranzaDto
            {
                IdPago = x.IdPago,
                Monto = x.MontoAbonado,
                Fecha = x.FechaPago ?? DateTime.MinValue,
                MetodoPago = x.IdFormaPagoNavigation?.Descripcion,
                Referencia = x.NroComprobanteBanco,
                Observaciones = x.Observaciones,
                Estado = x.Estado,
                ConceptoDeuda = x.IdDeudaNavigation != null 
                    ? $"{x.IdDeudaNavigation.MesPeriodo}/{x.IdDeudaNavigation.AnioPeriodo}" 
                    : string.Empty
            });
            return Result.Ok(result);
        }
    }
}
