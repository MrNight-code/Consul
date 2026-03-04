using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.IO;
using System;
using Consulcon.Infrastructure.Services.Facturacion; // Added correct namespace
using Consulcon.Domain.Entities.Facturacion;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Entities.Contratos;
using QuestPDF.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Consulcon.Infrastructure.Persistence;

namespace Consulcon.IntegrationTests.Services
{
    // Important: QuestPDF License must be set in global test setup or here
    public class ReceiptGenerationServiceTests : IDisposable
    {
        private readonly ReceiptGenerationService _service;
        private readonly string _outputFolder = "GeneratedReceipts";
        private readonly ConsulconDbContext _dbContext;

        public ReceiptGenerationServiceTests()
        {
            // Configure License for Tests
            QuestPDF.Settings.License = LicenseType.Community;

            // Setup In-Memory Database
            var options = new DbContextOptionsBuilder<ConsulconDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _dbContext = new ConsulconDbContext(options);

            _service = new ReceiptGenerationService(_dbContext);
        }

        [Fact]
        public async Task GenerateReceiptAsync_ShouldUpdateTransactionAndCreateFile()
        {
            // Arrange (Seed Hierarchy)
            var propiedad = new Propiedad { IdPropiedad = 1, CodigoUnidad = "U-101", NombreFuncional = "U-101" };
            var contrato = new Contrato { IdContrato = 1, IdPropiedadNavigation = propiedad };
            var deuda = new DeudaCabecera 
            { 
                IdDeuda = 1, 
                IdContratoNavigation = contrato, 
                AnioPeriodo = 2024, 
                MesPeriodo = 3 
            };
            
            var transaccion = new TransaccionPago 
            { 
                IdPago = 1, 
                MontoAbonado = 1500.50m, 
                IdDeudaNavigation = deuda,
                FechaPago = DateTime.Now
            };

            _dbContext.Propiedads.Add(propiedad);
            _dbContext.Contratos.Add(contrato);
            _dbContext.DeudaCabeceras.Add(deuda);
            _dbContext.TransaccionPagos.Add(transaccion);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GenerateReceiptAsync(transaccion.IdPago);

            // Assert
            // 1. Entity Integrity
            result.Should().NotBeNull();
            result.IdPago.Should().Be(transaccion.IdPago);
            
            // 2. Server Timestamp Validity
            result.FechaRecibo.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

            // 3. File Existence
            result.ReciboUrl.Should().NotBeNullOrEmpty();
            File.Exists(result.ReciboUrl).Should().BeTrue();
            
            // 4. File Content Check
            var fileInfo = new FileInfo(result.ReciboUrl);
            fileInfo.Length.Should().BeGreaterThan(1000);
        }

        public void Dispose()
        {
            // Cleanup: Delete generated files after test
            if (Directory.Exists(_outputFolder))
            {
                try { Directory.Delete(_outputFolder, true); } catch { }
            }
            GC.SuppressFinalize(this);
        }
    }
}
