using Consulcon.Application.DTOs;
using Consulcon.Infrastructure.Services;
using Consulcon.Infrastructure.Persistence;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Entities.Contratos;
using Consulcon.Domain.Entities.General;
using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consulcon.IntegrationTests.Services
{
    public class CobranzaServiceTests : IDisposable
    {
        private readonly ConsulconDbContext _context;
        private readonly CobranzaService _service;

        public CobranzaServiceTests()
        {
            var options = new DbContextOptionsBuilder<ConsulconDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ConsulconDbContext(options);
            _service = new CobranzaService(_context);
        }

        [Fact]
        public async Task RegistrarCobranza_ShouldUpdateBalanceAndApplyFIFO()
        {
            // Arrange
            // 1. Setup Propiedad
            var prop = new Propiedad { IdPropiedad = 1, CodigoUnidad = "U-101", SaldoDeudor = 200m, Activo = true };
            _context.Propiedads.Add(prop);

            // 1.1 Setup Dependencies (FormaPago, Persona, Banco)
            _context.FormaPagos.Add(new FormaPago { IdFormaPago = 1, Descripcion = "Efectivo", IdCuentaContableAsociada = 1 });
            _context.Personas.Add(new Persona { IdPersona = 1, NombreCompleto = "Test User", Ci = "123", Sexo = "M" });
            _context.Bancos.Add(new Banco { IdBanco = 1, NombreEntidad = "Banco Test", NumeroCuenta = "123", Activo = true, IdCuentaContableAsociada = 1 });
            // PlanCuentas needed for FKs above? Minimally:
            _context.PlanCuentas.Add(new PlanCuenta { IdCuenta = 1, CodigoCuenta = "1.1", Nombre = "Caja", EsImputable = true });

            // 2. Setup Contrato linked to Propiedad
            var contrato = new Contrato { IdContrato = 1, IdPropiedad = 1, FechaInicio = new DateOnly(2024,1,1), MontoExpensaPactada = 100, Estado = "VIGENTE" };
            _context.Contratos.Add(contrato);
            
            // 2.1 Setup ContratoParticipante (The Payer)
            _context.ContratoParticipantes.Add(new ContratoParticipante 
            { 
                IdContrato = 1, 
                IdPersona = 1, 
                RolContrato = "Titular", 
                Activo = true 
            });

            // 3. Setup Debts (Oldest first)
            var deudaOld = new DeudaCabecera 
            { 
                IdDeuda = 1, IdContrato = 1, 
                FechaVencimiento = new DateOnly(2024, 1, 10), 
                TotalDeuda = 100m, TotalPagado = 0, EstadoPago = "PENDIENTE" 
            };
            var deudaNew = new DeudaCabecera 
            { 
                IdDeuda = 2, IdContrato = 1, 
                FechaVencimiento = new DateOnly(2024, 2, 10), 
                TotalDeuda = 100m, TotalPagado = 0, EstadoPago = "PENDIENTE" 
            };
            _context.DeudaCabeceras.AddRange(deudaOld, deudaNew);
            await _context.SaveChangesAsync();

            // Act: Pay $150
            var request = new CobranzaRequest 
            { 
                UnitId = 1, 
                Monto = 150m, 
                IdFormaPago = 1, 
                NroReferencia = "REF001",
                IdBancoDestino = 1 // Valid Banco ID from setup
            };
            var result = await _service.RegistrarCobranzaAsync(request);

            // Assert
            result.IsSuccess.Should().BeTrue();

            // Check Propiedad Balance ($200 - $150 = $50)
            var propDb = await _context.Propiedads.FindAsync(1);
            propDb.Should().NotBeNull();
            propDb!.SaldoDeudor.Should().Be(50m);

            // Check FIFO logic
            var deudaOldDb = await _context.DeudaCabeceras.FindAsync(1);
            deudaOldDb.Should().NotBeNull();
            deudaOldDb!.EstadoPago.Should().Be("PAGADO");
            deudaOldDb.TotalPagado.Should().Be(100m);

            var deudaNewDb = await _context.DeudaCabeceras.FindAsync(2);
            deudaNewDb.Should().NotBeNull();
            deudaNewDb!.EstadoPago.Should().Be("PARCIAL");
            deudaNewDb.TotalPagado.Should().Be(50m); // Remaining $50 applied here

            // Check Transactions Created
            var pagos = await _context.TransaccionPagos.ToListAsync();
            pagos.Should().HaveCount(2); // One for each debt
            pagos.Sum(p => p.MontoAbonado).Should().Be(150m);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
