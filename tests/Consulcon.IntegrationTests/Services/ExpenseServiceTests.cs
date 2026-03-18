using Consulcon.Application.DTOs.Contabilidad.Expenses;
using Consulcon.Application.Interfaces;
using Consulcon.Domain.Common;
using Consulcon.Infrastructure.Services.Contabilidad;
using Consulcon.Infrastructure.Persistence;
using Consulcon.Domain.Entities.General;
using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using System;
using System.Threading.Tasks;

namespace Consulcon.IntegrationTests.Services
{
    public class ExpenseServiceTests : IDisposable
    {
        private readonly ConsulconDbContext _context;
        private readonly ExpenseService _service;

        private class DummyExpenseCalculationService : IExpenseCalculationService
        {
            public System.Collections.Generic.List<Consulcon.Domain.Entities.Contabilidad.UnitDebtDistribution> CalculateDistribution(
                Consulcon.Domain.Entities.Contabilidad.Egreso egreso,
                System.Collections.Generic.List<Consulcon.Domain.Entities.Inmuebles.Propiedad> propiedades,
                bool validarPorcentajeTotal = true,
                bool esMontoFijoPorUnidad = false)
            {
                return new System.Collections.Generic.List<Consulcon.Domain.Entities.Contabilidad.UnitDebtDistribution>();
            }
        }

        public ExpenseServiceTests()
        {
            var options = new DbContextOptionsBuilder<ConsulconDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ConsulconDbContext(options);
            _service = new ExpenseService(_context, null!, new DummyExpenseCalculationService());
        }

        [Fact]
        public async Task RegisterExpense_ShouldSucceed_WhenBalanceIsSufficient()
        {
            // Arrange
            var bankId = 1;
            var initialBalance = 1000m;
            var expenseAmount = 200m;

            _context.Bancos.Add(new Banco { IdBanco = bankId, NombreEntidad = "Bank A", Saldo = initialBalance, Tipo = "BANCO", Activo = true });
            await _context.SaveChangesAsync();

            var cmd = new RegisterExpenseCommand
            {
                AccountId = bankId,
                Amount = expenseAmount,
                Description = "Test Expense",
                ExpenseDate = DateTime.UtcNow.AddMinutes(-1),
                CondominioId = 10,
                CategoryId = 1,
                PaymentMethodId = 1
            };

            // Act
            var result = await _service.RegisterExpenseAsync(cmd, userId: 99);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            // Verify Bank Balance
            var bank = await _context.Bancos.FindAsync(bankId);
            bank.Should().NotBeNull();
            bank.Saldo.Should().Be(initialBalance - expenseAmount);

            // Verify Expense Created
            var expense = await _context.Egresos.FindAsync(result.Value);
            expense.Should().NotBeNull();
            expense.MontoTotal.Should().Be(expenseAmount);
            expense.IdCondominio.Should().Be(10);
            expense.IdUsuarioRegistro.Should().Be(99);

            // Verify Transaction History
            var history = await _context.AccountTransactionHistories.FirstOrDefaultAsync(h => h.ExpenseId == expense.IdEgreso);
            history.Should().NotBeNull();
            history.Amount.Should().Be(-expenseAmount);
            history.AccountId.Should().Be(bankId);
        }

        [Fact]
        public async Task RegisterExpense_ShouldFail_WhenBalanceInsufficient()
        {
            // Arrange
            var bankId = 2;
            _context.Bancos.Add(new Banco { IdBanco = bankId, NombreEntidad = "Bank B", Saldo = 100m, Tipo = "BANCO", Activo = true });
            await _context.SaveChangesAsync();

            var cmd = new RegisterExpenseCommand
            {
                AccountId = bankId,
                Amount = 150m, // More than balance
                ExpenseDate = DateTime.UtcNow
            };

            // Act
            var result = await _service.RegisterExpenseAsync(cmd, userId: 99);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Contain("Saldo insuficiente");

            // Verify No Changes
            var bank = await _context.Bancos.FindAsync(bankId);
            bank.Should().NotBeNull();
            bank.Saldo.Should().Be(100m);
            (await _context.Egresos.CountAsync()).Should().Be(0);
        }

        public void Dispose()
        {
             _context.Database.EnsureDeleted();
             _context.Dispose();
             GC.SuppressFinalize(this);
        }
    }
}
