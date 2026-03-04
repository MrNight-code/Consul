using Consulcon.Application.DTOs;
using Consulcon.Infrastructure.Services;
using Consulcon.Infrastructure.Persistence;
using Consulcon.Infrastructure.Persistence.Repositories; // Added
using Consulcon.Domain.Entities.General;
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Entities.Contabilidad; // Added for Egreso
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using System;
using System.Threading.Tasks;

namespace Consulcon.IntegrationTests.Services
{
    public class AccountServiceTests : IDisposable
    {
        private readonly ConsulconDbContext _context;
        private readonly AccountService _service;

        public AccountServiceTests()
        {
            var options = new DbContextOptionsBuilder<ConsulconDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ConsulconDbContext(options);
            var egresoRepository = new EfRepository<Egreso>(_context);
            _service = new AccountService(_context, egresoRepository, null!);
        }

        [Fact]
        public async Task CRUD_ShouldWorkCorrectly()
        {
            // Create
            var dto = new AccountDto { Name = "Caja Principal", Type = "EFECTIVO", IsActive = true };
            var createResult = await _service.CreateAccountAsync(dto);
            createResult.IsSuccess.Should().BeTrue();
            int newId = createResult.Value;

            // Read
            var getResult = await _service.GetAccountByIdAsync(newId);
            getResult.Value.Name.Should().Be("Caja Principal");

            // Update
            dto.Name = "Caja General";
            await _service.UpdateAccountAsync(newId, dto);
            var updated = await _context.Bancos.FindAsync(newId);
            updated.Should().NotBeNull();
            updated!.NombreEntidad.Should().Be("Caja General");

            // Delete (Safe because no payments yet)
            var deleteResult = await _service.DeleteAccountAsync(newId);
            deleteResult.IsSuccess.Should().BeTrue();
            (await _context.Bancos.FindAsync(newId)).Should().BeNull();
        }

        [Fact]
        public async Task Delete_ShouldFail_IfHasPayments()
        {
            // Arrange
            var banco = new Banco { IdBanco = 1, NombreEntidad = "Banco A", Activo = true, Tipo = "BANCO" };
            _context.Bancos.Add(banco);

            // Add Payment linked to this bank
            var pago = new TransaccionPago { IdPago = 1, IdBancoDestino = 1, MontoAbonado = 100 };
            _context.TransaccionPagos.Add(pago);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteAccountAsync(1);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("asociadas");
        }

        [Fact]
        public async Task GetAll_ShouldFilterActive()
        {
             _context.Bancos.Add(new Banco { IdBanco = 1, NombreEntidad = "A", Activo = true, Tipo = "BANCO" });
             _context.Bancos.Add(new Banco { IdBanco = 2, NombreEntidad = "B", Activo = false, Tipo = "BANCO" });
             await _context.SaveChangesAsync();

             var result = await _service.GetAllAccountsAsync(activeOnly: true);
             result.Value.Should().HaveCount(1);
             result.Value[0].Name.Should().Be("A");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
