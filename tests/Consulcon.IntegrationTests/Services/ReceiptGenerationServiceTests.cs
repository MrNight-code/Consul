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

using Consulcon.Application.DTOs.Facturacion;
using Consulcon.Domain.Interfaces;
using Moq;
using System.Linq;

using Consulcon.Domain.Entities.General;

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

            var mockRepo = new Mock<IRepository<TransaccionPago>>();
            _service = new ReceiptGenerationService(_dbContext, mockRepo.Object);
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
                FechaPago = DateTime.Now,
                IdBancoDestinoNavigation = new Banco { NombreEntidad = "Test Bank" },
                IdFormaPagoNavigation = new FormaPago { Descripcion = "Transfer" },
                IdPersonaPagadorNavigation = new Persona { NombreCompleto = "Test User" }
            };

            _dbContext.Propiedads.Add(propiedad);
            _dbContext.Contratos.Add(contrato);
            _dbContext.DeudaCabeceras.Add(deuda);
            _dbContext.TransaccionPagos.Add(transaccion);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _service.GenerateReceiptAsync(transaccion.IdPago);

            // Assert
            result.Should().NotBeNull();
            result.FechaRecibo.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            result.ReciboUrl.Should().NotBeNullOrEmpty();
            File.Exists(result.ReciboUrl).Should().BeTrue();
        }

        [Fact]
        public async Task GenerateBatchReceiptsPdfAsync_ShouldReturnByteArray()
        {
            // Arrange
            var start = DateTime.Now.AddDays(-1);
            var end = DateTime.Now.AddDays(1);
            
            var p1 = new Propiedad { IdPropiedad = 10, CodigoUnidad = "U-10" };
            var c1 = new Contrato { IdContrato = 10, IdPropiedadNavigation = p1 };
            var d1 = new DeudaCabecera { IdDeuda = 10, IdContratoNavigation = c1, MesPeriodo = 1, AnioPeriodo = 2024 };
            var t1 = new TransaccionPago 
            { 
                IdPago = 10, FechaPago = DateTime.Now, MontoAbonado = 100, IdDeudaNavigation = d1,
                IdBancoDestinoNavigation = new Banco { NombreEntidad = "B1" },
                IdFormaPagoNavigation = new FormaPago { Descripcion = "F1" },
                IdPersonaPagadorNavigation = new Persona { NombreCompleto = "P1" }
            };

            var d2 = new DeudaCabecera { IdDeuda = 11, IdContratoNavigation = c1, MesPeriodo = 2, AnioPeriodo = 2024 };
            var t2 = new TransaccionPago 
            { 
                IdPago = 11, FechaPago = DateTime.Now, MontoAbonado = 200, IdDeudaNavigation = d2,
                IdBancoDestinoNavigation = new Banco { NombreEntidad = "B2" },
                IdFormaPagoNavigation = new FormaPago { Descripcion = "F2" },
                IdPersonaPagadorNavigation = new Persona { NombreCompleto = "P2" }
            };

            _dbContext.TransaccionPagos.AddRange(t1, t2);
            await _dbContext.SaveChangesAsync();

            var request = new BatchReceiptRequestDto { StartDate = start, EndDate = end };

            // Act
            var pdfBytes = await _service.GenerateBatchReceiptsPdfAsync(request);

            // Assert
            pdfBytes.Should().NotBeNull();
            pdfBytes.Length.Should().BeGreaterThan(0);
        }

        public void Dispose()
        {
            if (Directory.Exists(_outputFolder))
            {
                try { Directory.Delete(_outputFolder, true); } catch { }
            }
            GC.SuppressFinalize(this);
        }
    }
}
