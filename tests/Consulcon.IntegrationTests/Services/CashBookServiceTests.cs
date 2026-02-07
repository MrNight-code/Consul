using Consulcon.Application.DTOs.Contabilidad.CashBook;
using Consulcon.Infrastructure.Services.Contabilidad;
using Consulcon.Infrastructure.Persistence;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Entities.General;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Entities.Contratos;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Consulcon.IntegrationTests.Services;

public class CashBookServiceTests : IDisposable
{
    private readonly ConsulconDbContext _context;
    private readonly CashBookService _service;

    public CashBookServiceTests()
    {
        var options = new DbContextOptionsBuilder<ConsulconDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new ConsulconDbContext(options);
        _service = new CashBookService(_context);
        
        SetupTestData();
    }

    private void SetupTestData()
    {
        // Common dependencies
        _context.PlanCuentas.Add(new PlanCuenta { IdCuenta = 1, CodigoCuenta = "1.1", Nombre = "Caja", EsImputable = true });
        _context.FormaPagos.Add(new FormaPago { IdFormaPago = 1, Descripcion = "Efectivo", IdCuentaContableAsociada = 1 });
        _context.Personas.Add(new Persona { IdPersona = 1, NombreCompleto = "Test User", Ci = "123", Sexo = "M" });
        
        // Accounts (Bancos)
        _context.Bancos.Add(new Banco { IdBanco = 1, NombreEntidad = "Banco Nacional", NumeroCuenta = "001", Activo = true, Saldo = 10000 });
        _context.Bancos.Add(new Banco { IdBanco = 2, NombreEntidad = "Banco Regional", NumeroCuenta = "002", Activo = true, Saldo = 5000 });
        
        // Property and Contract for Cobranzas
        var prop = new Propiedad { IdPropiedad = 1, CodigoUnidad = "U-101", SaldoDeudor = 0, Activo = true };
        _context.Propiedads.Add(prop);
        
        var contrato = new Contrato { IdContrato = 1, IdPropiedad = 1, FechaInicio = new DateOnly(2025, 1, 1), MontoExpensaPactada = 100, Estado = "VIGENTE" };
        _context.Contratos.Add(contrato);
        
        // Debts for TransaccionPago
        _context.DeudaCabeceras.Add(new DeudaCabecera { IdDeuda = 1, IdContrato = 1, MesPeriodo = 1, AnioPeriodo = 2026, TotalDeuda = 100, EstadoPago = "PAGADO" });
        _context.DeudaCabeceras.Add(new DeudaCabecera { IdDeuda = 2, IdContrato = 1, MesPeriodo = 2, AnioPeriodo = 2026, TotalDeuda = 100, EstadoPago = "PAGADO" });
        
        // Income Transactions (TransaccionPago) - Type=IN
        _context.TransaccionPagos.AddRange(
            new TransaccionPago { IdPago = 1, IdDeuda = 1, IdBancoDestino = 1, IdFormaPago = 1, IdPersonaPagador = 1, MontoAbonado = 500, FechaPago = new DateTime(2026, 1, 5), Estado = "CONFIRMADO", NroComprobanteBanco = "INC-001" },
            new TransaccionPago { IdPago = 2, IdDeuda = 2, IdBancoDestino = 1, IdFormaPago = 1, IdPersonaPagador = 1, MontoAbonado = 300, FechaPago = new DateTime(2026, 1, 15), Estado = "CONFIRMADO", NroComprobanteBanco = "INC-002" },
            new TransaccionPago { IdPago = 3, IdDeuda = 1, IdBancoDestino = 2, IdFormaPago = 1, IdPersonaPagador = 1, MontoAbonado = 200, FechaPago = new DateTime(2026, 1, 20), Estado = "CONFIRMADO", NroComprobanteBanco = "INC-003" }, // Different account
            new TransaccionPago { IdPago = 4, IdDeuda = 2, IdBancoDestino = 1, IdFormaPago = 1, IdPersonaPagador = 1, MontoAbonado = 100, FechaPago = new DateTime(2026, 1, 25), Estado = "ANULADO", NroComprobanteBanco = "INC-VOID" } // Voided
        );
        
        // Expenses (Egreso) - Type=OUT
        _context.Egresos.AddRange(
            new Egreso { IdEgreso = 1, IdCondominio = 1, IdBancoOrigen = 1, IdAutorizacion = 1, IdFormaPago = 1, IdUsuarioRegistro = 1, MontoTotal = 150, Concepto = "Mantenimiento", FechaEgreso = new DateTime(2026, 1, 10) },
            new Egreso { IdEgreso = 2, IdCondominio = 1, IdBancoOrigen = 1, IdAutorizacion = 1, IdFormaPago = 1, IdUsuarioRegistro = 1, MontoTotal = 200, Concepto = "Limpieza", FechaEgreso = new DateTime(2026, 1, 22), NroFacturaProveedor = "F-001" }
        );
        
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetCashBook_ShouldCalculateCorrectBalance_WithMixedTransactions()
    {
        // Arrange
        var query = new CashBookQuery
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 31)
        };

        // Act
        var result = await _service.GetCashBookAsync(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var cashBook = result.Value;
        cashBook.Entries.Should().HaveCount(5); // 3 income (excl. voided) + 2 expenses
        cashBook.InitialBalance.Should().Be(0); // No transactions before Jan 1
        
        // Verify order (chronological)
        cashBook.Entries[0].Date.Should().Be(new DateTime(2026, 1, 5)); // Income 500
        cashBook.Entries[1].Date.Should().Be(new DateTime(2026, 1, 10)); // Expense -150
        
        // Verify running balance
        // INC +500 = 500, EXP -150 = 350, INC +300 = 650, INC +200 = 850, EXP -200 = 650
        cashBook.FinalBalance.Should().Be(650);
    }

    [Fact]
    public async Task GetCashBook_ShouldExcludeVoidedFromCalculation_WhenIncludeVoidedIsFalse()
    {
        // Arrange
        var query = new CashBookQuery
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 31),
            IncludeVoided = false
        };

        // Act
        var result = await _service.GetCashBookAsync(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Entries.Should().NotContain(e => e.IsVoided);
        result.Value.Entries.Should().HaveCount(5); // 3 confirmed income + 2 expenses
    }

    [Fact]
    public async Task GetCashBook_ShouldShowVoidedVisually_WhenIncludeVoidedIsTrue()
    {
        // Arrange
        var query = new CashBookQuery
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 31),
            IncludeVoided = true
        };

        // Act
        var result = await _service.GetCashBookAsync(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Entries.Should().Contain(e => e.IsVoided);
        result.Value.Entries.Should().HaveCount(6); // 4 income (incl. voided) + 2 expenses
        
        // Voided should NOT affect balance
        var voidedEntry = result.Value.Entries.First(e => e.IsVoided);
        voidedEntry.Amount.Should().Be(100); // Original amount
        
        // Final balance should still be 650 (voided excluded from calculation)
        result.Value.FinalBalance.Should().Be(650);
    }

    [Fact]
    public async Task GetCashBook_ShouldFilterByAccount_WhenFinancialAccountIdProvided()
    {
        // Arrange
        var query = new CashBookQuery
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 31),
            FinancialAccountId = 1 // Only Banco Nacional
        };

        // Act
        var result = await _service.GetCashBookAsync(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Entries.Should().OnlyContain(e => e.AccountId == 1);
        result.Value.Entries.Should().HaveCount(4); // 2 income + 2 expenses (all on account 1)
        
        // Balance for account 1 only: +500 -150 +300 -200 = 450
        result.Value.FinalBalance.Should().Be(450);
    }

    [Fact]
    public async Task GetCashBook_ShouldPaginateCorrectly_PreservingRunningBalance()
    {
        // Arrange
        var query = new CashBookQuery
        {
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 31),
            Page = 2,
            PageSize = 2
        };

        // Act
        var result = await _service.GetCashBookAsync(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Entries.Should().HaveCount(2);
        result.Value.Page.Should().Be(2);
        result.Value.TotalPages.Should().Be(3); // 5 entries / 2 per page = 3 pages
        
        // First page: entries 0,1 (500 - 150 = 350)
        // Second page starts with running balance of 350
        result.Value.Entries[0].Balance.Should().Be(650); // 350 + 300
        result.Value.Entries[1].Balance.Should().Be(850); // 650 + 200
    }

    [Fact]
    public async Task GetCashBook_ShouldCalculateInitialBalance_FromPriorTransactions()
    {
        // Arrange: Query for February, so January transactions form the initial balance
        var query = new CashBookQuery
        {
            StartDate = new DateTime(2026, 2, 1),
            EndDate = new DateTime(2026, 2, 28)
        };

        // Act
        var result = await _service.GetCashBookAsync(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Entries.Should().BeEmpty(); // No February transactions
        result.Value.InitialBalance.Should().Be(650); // Sum of all January confirmed transactions
    }

    [Fact]
    public async Task GetCashBook_ShouldFail_WhenStartDateAfterEndDate()
    {
        // Arrange
        var query = new CashBookQuery
        {
            StartDate = new DateTime(2026, 2, 1),
            EndDate = new DateTime(2026, 1, 1)
        };

        // Act
        var result = await _service.GetCashBookAsync(query);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("fecha de inicio");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
