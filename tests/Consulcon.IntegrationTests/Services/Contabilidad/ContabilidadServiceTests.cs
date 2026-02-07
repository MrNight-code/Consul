using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Services.Contabilidad;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Entities.General;
using Consulcon.Infrastructure.Persistence;
using Consulcon.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using FluentAssertions;
using Xunit;
using System;
using System.Threading.Tasks;

namespace Consulcon.IntegrationTests.Services.Contabilidad;

public class ContabilidadServiceTests : IDisposable
{
    private readonly ConsulconDbContext _context;
    private readonly ContabilidadService _service;

    public ContabilidadServiceTests()
    {
        var options = new DbContextOptionsBuilder<ConsulconDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ConsulconDbContext(options);

        var planRepo = new EfRepository<PlanCuenta>(_context);
        var asientoRepo = new EfRepository<AsientoContable>(_context);
        var detalleRepo = new EfRepository<AsientoDetalle>(_context);
        var autoRepo = new EfRepository<AutorizacionGasto>(_context);
        var egresoRepo = new EfRepository<Egreso>(_context);
        var bancoRepo = new EfRepository<Banco>(_context);
        var unitOfWork = new UnitOfWork(_context);
        var logger = NullLogger<ContabilidadService>.Instance;

        _service = new ContabilidadService(planRepo, asientoRepo, detalleRepo, autoRepo, egresoRepo, bancoRepo, unitOfWork, logger);
    }

    [Fact]
    public async Task VoidExpenseAsync_ShouldVoidExpenseAndRevertBalance()
    {
        // Arrange
        var expenseAmount = 100m;
        var initialBalance = 500m;
        
        var banco = new Banco 
        { 
            IdBanco = 1, 
            NombreEntidad = "Test Bank", 
            Saldo = initialBalance,
            Activo = true,
            NumeroCuenta = "123",
            Moneda = "USD",
            Tipo = "BANCO"
        };
        _context.Bancos.Add(banco);

        var egreso = new Egreso
        {
            IdEgreso = 1,
            IdBancoOrigen = 1,
            MontoTotal = expenseAmount,
            Concepto = "Expense to be voided",
            FechaEgreso = DateTime.Now,
            IdCondominio = 1,
            IdAutorizacion = 1, 
            IdFormaPago = 1,
            IdUsuarioRegistro = 1
        };
        _context.Egresos.Add(egreso);
        await _context.SaveChangesAsync();

        var request = new VoidExpenseRequest { Reason = "Mistake in amounts > 10 chars" };

        // Act
        var result = await _service.VoidExpenseAsync(1, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedEgreso = await _context.Egresos.FindAsync(1);
        updatedEgreso.Should().NotBeNull();
        updatedEgreso!.Concepto.Should().StartWith("[ANULADO]");
        updatedEgreso.Concepto.Should().Contain(request.Reason);

        var updatedBanco = await _context.Bancos.FindAsync(1);
        updatedBanco.Should().NotBeNull();
        updatedBanco!.Saldo.Should().Be(initialBalance + expenseAmount);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
