using Consulcon.Application.DTOs.Contabilidad.Expenses;
using Consulcon.Application.Interfaces;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Specifications;
using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Consulcon.Domain.Interfaces;
using System;
using System.Threading.Tasks;


namespace Consulcon.Infrastructure.Services.Contabilidad
{
    public class ExpenseService(ConsulconDbContext context, IRepository<Egreso> repository, IExpenseCalculationService calculationService) : IExpenseService
    {
        public async Task<Result<int>> RegisterExpenseAsync(RegisterExpenseCommand cmd, int userId)
        {
            // Validations
            if (cmd.Amount <= 0.01m)
                return Result.Fail<int>("El monto debe ser mayor a 0.01.");

            if (cmd.ExpenseDate > DateTime.UtcNow)
                return Result.Fail<int>("La fecha del egreso no puede ser futura.");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                // 1. Get Account
                var account = await context.Bancos.FindAsync(cmd.AccountId);
                if (account == null)
                    return Result.Fail<int>("La cuenta especificada no existe.");

                // Validate Balance
                if (account.Saldo < cmd.Amount)
                     return Result.Fail<int>("Saldo insuficiente en la cuenta seleccionada.");

                // 2. Debit Account
                account.Debit(cmd.Amount);
                
                // 3. Create Expense
                var expense = new Egreso
                {
                    IdCondominio = cmd.CondominioId,
                    IdBancoOrigen = cmd.AccountId,
                    IdAutorizacion = cmd.CategoryId,
                    IdFormaPago = cmd.PaymentMethodId,
                    Concepto = cmd.Description,
                    MontoTotal = cmd.Amount,
                    FechaEgreso = cmd.ExpenseDate,
                    NroFacturaProveedor = cmd.InvoiceNumber,
                    IdProveedor = cmd.ProviderId,
                    IdUsuarioRegistro = userId,
                    // Default/Nulls
                    IdPersonaBeneficiario = null
                };

                context.Egresos.Add(expense);
                await context.SaveChangesAsync();

                // 4. Create Transaction History
                var history = new AccountTransactionHistory
                {
                    Id = Guid.NewGuid(),
                    AccountId = cmd.AccountId,
                    ExpenseId = expense.IdEgreso,
                    Amount = -cmd.Amount, // Negative for expense
                    Date = DateTime.UtcNow,
                    Description = $"Egreso #{expense.IdEgreso}: {cmd.Description}",
                    ReferenceId = expense.IdEgreso.ToString()
                };
                context.AccountTransactionHistories.Add(history);
                await context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Result.Ok(expense.IdEgreso);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Fail<int>($"Error al registrar el gasto: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<EgresoDto>>> GetPagedAsync(int idCondominio, PaginationParams p)
        {
            var spec = new EgresoWithFiltersSpec(p, idCondominio);

            var pagedData = await repository.GetPagedAsync(spec, p.PageNumber, p.PageSize);

            var result = pagedData.Map(x => new EgresoDto
            {
                Id = x.IdEgreso,
                IdCondominio = x.IdCondominio,
                IdProveedor = x.IdProveedor,
                ProveedorNombre = x.IdProveedorNavigation?.RazonSocial,
                IdPersonaBeneficiario = x.IdPersonaBeneficiario,
                BeneficiarioNombre = x.IdPersonaBeneficiarioNavigation?.NombreCompleto, 
                IdAutorizacion = x.IdAutorizacion,
                IdBancoOrigen = x.IdBancoOrigen,
                BancoNombre = x.IdBancoOrigenNavigation?.NombreEntidad,
                IdFormaPago = x.IdFormaPago,
                Concepto = x.Concepto,
                MontoTotal = x.MontoTotal,
                FechaEgreso = x.FechaEgreso,
                NroFacturaProveedor = x.NroFacturaProveedor
            });

            return Result.Ok(result);
        }
        public async Task<Result<System.Collections.Generic.List<ExpensaDistribucionDto>>> SimularCalculoExpensasAsync(ExpensaSimulacionRequestDto request)
        {
            try
            {
                // 1. Validate Condominio
                if (!request.FkCondominio.HasValue)
                    return Result.Fail<System.Collections.Generic.List<ExpensaDistribucionDto>>("Se requiere el ID del Condominio.");

                // 2. Fetch active properties for the given Condominio
                var propiedades = await context.Propiedads
                    .Include(p => p.IdManzanoNavigation)
                    .Include(p => p.Contratos)
                        .ThenInclude(c => c.ContratoParticipantes)
                            .ThenInclude(cp => cp.IdPersonaNavigation) // For Propietario name
                    .Where(p => p.IdManzanoNavigation.IdCondominio == request.FkCondominio.Value && p.Activo == true)
                    .ToListAsync();

                System.Console.WriteLine($"[ExpenseService] Propiedades activas encontradas para Condominio {request.FkCondominio}: {propiedades.Count}");

                if (!propiedades.Any())
                    return Result.Fail<System.Collections.Generic.List<ExpensaDistribucionDto>>("No se encontraron propiedades activas para este condominio.");

                // 3. Create a dummy Egreso for calculation
                var egresoMoc = new Egreso { IdCondominio = request.FkCondominio.Value, MontoTotal = request.MontoTotal };

                // 4. Calculate Distribution
                var distribuciones = calculationService.CalculateDistribution(egresoMoc, propiedades, validarPorcentajeTotal: true, esMontoFijoPorUnidad: request.EsMontoFijoPorUnidad);


                // 5. Map to DTO
                var resultList = new System.Collections.Generic.List<ExpensaDistribucionDto>();
                foreach (var dist in distribuciones)
                {
                    var prop = propiedades.First(p => p.IdPropiedad == dist.IdPropiedad);
                    
                    // Find active contract and owner name
                    var activeContract = prop.Contratos
                        .OrderByDescending(c => c.FechaInicio)
                        .FirstOrDefault(c => c.Estado == "VIGENTE" || c.Estado == null);
                    
                    activeContract ??= prop.Contratos.OrderByDescending(c => c.FechaInicio).FirstOrDefault();

                    string propietario = "";
                    if (activeContract != null)
                    {
                        var titular = activeContract.ContratoParticipantes
                            .FirstOrDefault(cp => cp.RolContrato == "Titular") 
                            ?? activeContract.ContratoParticipantes.FirstOrDefault();
                        
                        if (titular != null && titular.IdPersonaNavigation != null)
                        {
                            propietario = titular.IdPersonaNavigation.NombreCompleto;
                        }
                    }

                    resultList.Add(new ExpensaDistribucionDto
                    {
                        Id = dist.IdPropiedad, // Using Propiedad ID as unique row ID for the grid
                        Unidad = prop.CodigoUnidad,
                        Propietario = propietario,
                        PorcentajeIncidencia = dist.PorcentajeAplicado,
                        MontoAPagar = dist.MontoCalculado,
                        FkPropiedad = prop.IdPropiedad,
                        FkContrato = activeContract?.IdContrato,
                        FechaCobro = null // Not registered yet
                    });
                }

                return Result.Ok(resultList);
            }
            catch (Exception ex)
            {
                return Result.Fail<System.Collections.Generic.List<ExpensaDistribucionDto>>($"Error en la simulación: {ex.Message}");
            }
        }
        public async Task<Result<System.Collections.Generic.List<SaldoUnidadDto>>> ObtenerSaldosUnidadesAsync(int fkCondominio)
        {
            try
            {
                var propiedades = await context.Propiedads
                    .Include(p => p.IdManzanoNavigation)
                    .Include(p => p.Contratos)
                        .ThenInclude(c => c.ContratoParticipantes)
                            .ThenInclude(cp => cp.IdPersonaNavigation)
                    .Where(p => p.IdManzanoNavigation.IdCondominio == fkCondominio && p.Activo == true)
                    .ToListAsync();
                    
                var resultList = new System.Collections.Generic.List<SaldoUnidadDto>();

                foreach (var prop in propiedades)
                {
                    var activeContract = prop.Contratos
                        .OrderByDescending(c => c.FechaInicio)
                        .FirstOrDefault(c => c.Estado == "VIGENTE" || c.Estado == null);
                    
                    activeContract ??= prop.Contratos.OrderByDescending(c => c.FechaInicio).FirstOrDefault();

                    string propietario = "";
                    if (activeContract != null)
                    {
                        var titular = activeContract.ContratoParticipantes
                            .FirstOrDefault(cp => cp.RolContrato == "Titular") 
                            ?? activeContract.ContratoParticipantes.FirstOrDefault();
                        
                        if (titular != null && titular.IdPersonaNavigation != null)
                        {
                            propietario = titular.IdPersonaNavigation.NombreCompleto;
                        }
                    }
                    
                    // The Frontend expects SaldoActual to be SaldoAFavor or positive representing credit.
                    // We only want to report if they have Saldo A Favor for reconciliation.
                    resultList.Add(new SaldoUnidadDto
                    {
                        FkPropiedad = prop.IdPropiedad,
                        Unidad = prop.CodigoUnidad,
                        Propietario = string.IsNullOrEmpty(propietario) ? null : propietario,
                        SaldoActual = prop.SaldoAFavor,
                        TienePropietario = !string.IsNullOrEmpty(propietario)
                    });
                }
                
                return Result.Ok(resultList);
            }
            catch (Exception ex)
            {
                return Result.Fail<System.Collections.Generic.List<SaldoUnidadDto>>($"Error al obtener saldos: {ex.Message}");
            }
        }

        public async Task<Result<ConciliacionExpensaDto>> ConciliarExpensasAsync(ConciliarExpensasRequestDto request)
        {
            try
            {
                var saldosResult = await ObtenerSaldosUnidadesAsync(request.FkCondominio);
                if (saldosResult.IsFailure)
                    return Result.Fail<ConciliacionExpensaDto>(saldosResult.Error);

                var saldos = saldosResult.Value;
                var saldosDict = saldos.ToDictionary(s => s.FkPropiedad, s => s);
                
                var excepciones = new System.Collections.Generic.List<UnidadExcepcionDto>();
                decimal totalSaldosAFavor = 0m;
                int unidadesSinPropietario = 0;

                foreach (var item in request.Distribucion)
                {
                    if (item.FkPropiedad.HasValue && saldosDict.TryGetValue(item.FkPropiedad.Value, out var saldo))
                    {
                        if (saldo.SaldoActual >= item.MontoAPagar)
                        {
                            var excepcion = new UnidadExcepcionDto
                            {
                                FkPropiedad = item.FkPropiedad.Value,
                                Unidad = item.Unidad,
                                Propietario = item.Propietario,
                                SaldoAFavor = saldo.SaldoActual,
                                MontoExpensa = item.MontoAPagar,
                                MontoNeto = saldo.SaldoActual - item.MontoAPagar,
                                TienePropietario = saldo.TienePropietario,
                                TipoExcepcion = "Pago Adelantado"
                            };
                            excepciones.Add(excepcion);
                            totalSaldosAFavor += item.MontoAPagar;
                        }

                        if (!saldo.TienePropietario)
                        {
                            unidadesSinPropietario++;
                        }
                    }
                }

                return Result.Ok(new ConciliacionExpensaDto
                {
                    MontoBruto = request.MontoTotal,
                    TotalSaldosAFavor = totalSaldosAFavor,
                    MontoNeto = request.MontoTotal - totalSaldosAFavor,
                    UnidadesConExcepcion = excepciones.Count,
                    UnidadesSinPropietario = unidadesSinPropietario,
                    Excepciones = excepciones
                });
            }
            catch (Exception ex)
            {
                return Result.Fail<ConciliacionExpensaDto>($"Error en la conciliación: {ex.Message}");
            }
        }

        public async Task<Result<GenerarDeudaResponseDto>> GenerarDeudaExpensasAsync(GenerarDeudaRequestDto request, int userId)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {

                // 1. Get properties
                var propiedades = await context.Propiedads
                    .Include(p => p.IdManzanoNavigation)
                    .Include(p => p.Contratos)
                    .Where(p => p.IdManzanoNavigation.IdCondominio == request.FkCondominio && p.Activo == true)
                    .ToListAsync();

                if (!propiedades.Any())
                    return Result.Fail<GenerarDeudaResponseDto>("No se encontraron propiedades activas para este condominio.");

                var servicioExpensa = await context.CatalogoServicios.FirstOrDefaultAsync(s => s.Nombre.Contains("Expensa"));
                if (servicioExpensa == null)
                    return Result.Fail<GenerarDeudaResponseDto>("No se configuró el servicio 'Expensas' en el Administrador de Catálogos.");

                // 1.c GAP DETECTION LOGIC
                // Buscar la última deuda de "Expensa" para este condominio
                var ultimaDeuda = await context.DeudaCabeceras
                    .Include(d => d.IdContratoNavigation.IdPropiedadNavigation.IdManzanoNavigation)
                    .Include(d => d.DeudaDetalles)
                    .Where(d => 
                        d.IdContratoNavigation.IdPropiedadNavigation.IdManzanoNavigation.IdCondominio == request.FkCondominio &&
                        d.DeudaDetalles.Any(det => det.Concepto.Contains("Expensas")))
                    .OrderByDescending(d => d.AnioPeriodo)
                    .ThenByDescending(d => d.MesPeriodo)
                    .FirstOrDefaultAsync();

                DateTime fechaSolicitada = new DateTime(request.Ano, request.Mes, 1);

                if (ultimaDeuda != null)
                {
                    // Calculamos matemáticamente la fecha que el motor espera ahora
                    DateTime ultimaFecha = new DateTime(ultimaDeuda.AnioPeriodo, ultimaDeuda.MesPeriodo, 1);
                    DateTime fechaEsperada = ultimaFecha.AddMonths(1);
                    
                    if (fechaSolicitada > fechaEsperada)
                    {
                        // Gap detectado
                        return Result.Fail<GenerarDeudaResponseDto>(
                            $"GAP_DETECTED|{fechaEsperada.Month}|{fechaEsperada.Year}|El último periodo cobrado fue {ultimaFecha.Month:00}/{ultimaFecha.Year}. Debe cobrar {fechaEsperada.Month:00}/{fechaEsperada.Year} primero.");
                    }
                    
                    if (fechaSolicitada <= ultimaFecha)
                    {
                        // El periodo solicitado ya fue generado previamente
                        return Result.Fail<GenerarDeudaResponseDto>(
                            $"El periodo {request.Mes:00}/{request.Ano} ya fue generado anteriormente para este condominio.");
                    }
                }
                else
                {
                    // Opción 2: No existe ninguna expensa previa para este condominio.
                    // Permitir al usuario generar libremente el primer mes que desee.
                    // El gap filler solo se activará a partir del segundo mes en adelante.
                }

                // 2. Mock Egreso for Distribution
                var egresoMoc = new Egreso { IdCondominio = request.FkCondominio, MontoTotal = request.MontoTotal };
                var distribuciones = calculationService.CalculateDistribution(egresoMoc, propiedades, validarPorcentajeTotal: true, esMontoFijoPorUnidad: request.EsMontoFijoPorUnidad);

                int deudasGeneradas = 0;

                // 3. Generate Deudas for active contracts only
                foreach (var dist in distribuciones)
                {
                    var prop = propiedades.First(p => p.IdPropiedad == dist.IdPropiedad);
                    var activeContract = prop.Contratos
                        .OrderByDescending(c => c.FechaInicio)
                        .FirstOrDefault(c => c.Estado == "VIGENTE" || c.Estado == null);
                    
                    activeContract ??= prop.Contratos.OrderByDescending(c => c.FechaInicio).FirstOrDefault();

                    if (activeContract == null) 
                        continue; // No active contract, skip generating debt

                    decimal pagar = dist.MontoCalculado;
                    decimal deduccion = 0m;

                    // Support applying favorable balances immediately
                    if (request.AplicarSaldosAFavor && prop.SaldoAFavor > 0)
                    {
                        if (prop.SaldoAFavor >= pagar)
                        {
                            deduccion = pagar;
                            prop.SaldoAFavor -= pagar;
                        }
                        else
                        {
                            deduccion = prop.SaldoAFavor;
                            prop.SaldoAFavor = 0;
                        }
                    }

                    // Aumentar el saldo deudor de la propiedad por la porción no cubierta por el saldo a favor
                    prop.SaldoDeudor += (pagar - deduccion);
                    
                    context.Propiedads.Update(prop);

                    var deuda = new Consulcon.Domain.Entities.Facturacion.DeudaCabecera
                    {
                        IdContrato = activeContract.IdContrato,
                        AnioPeriodo = request.Ano,
                        MesPeriodo = request.Mes,
                        FechaEmision = DateOnly.FromDateTime(DateTime.UtcNow),
                        FechaVencimiento = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                        TotalDeuda = pagar,
                        TotalPagado = deduccion,
                        EstadoPago = deduccion >= pagar ? "PAGADO" : (deduccion > 0 ? "PARCIAL" : "PENDIENTE"),
                        IdUsuarioGenerador = userId
                    };
                    
                    context.DeudaCabeceras.Add(deuda);
                    
                    // We must save to get the IdDeuda for the detail
                    await context.SaveChangesAsync();
                    
                    var detalle = new Consulcon.Domain.Entities.Facturacion.DeudaDetalle
                    {
                        IdDeuda = deuda.IdDeuda,
                        IdServicio = servicioExpensa.IdServicio,
                        Concepto = $"Expensas {request.Mes}/{request.Ano}",
                        MontoUnitario = pagar,
                        Cantidad = 1,
                        Subtotal = pagar
                    };

                    context.DeudaDetalles.Add(detalle);
                    deudasGeneradas++;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Result.Ok(new GenerarDeudaResponseDto
                {
                    Exitoso = true,
                    Mensaje = $"Se generaron {deudasGeneradas} deudas correctamente."
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result.Fail<GenerarDeudaResponseDto>($"Error al generar la deuda: {ex.Message}");
            }
        }
    }
}
