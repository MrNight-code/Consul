using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System;
using Consulcon.Application.Services;
using Consulcon.Domain.Entities.Contabilidad;
using Consulcon.Domain.Entities.Inmuebles;

namespace Consulcon.IntegrationTests.Services
{
    public class ExpenseCalculationServiceTests
    {
        private readonly ExpenseCalculationService _service;

        public ExpenseCalculationServiceTests()
        {
            _service = new ExpenseCalculationService();
        }

        [Fact]
        public void CalculateDistribution_StandardCase_ShouldDistributeCorrectly()
        {
            // Arrange
            var egreso = new Egreso { IdEgreso = 1, MontoTotal = 10000m };
            var props = new List<Propiedad>
            {
                new Propiedad { IdPropiedad = 1, CodigoUnidad = "U-001", PorcentajeParticipacion = 5m, Activo = true },
                new Propiedad { IdPropiedad = 2, CodigoUnidad = "U-002", PorcentajeParticipacion = 95m, Activo = true }
             };

            // Act
            var result = _service.CalculateDistribution(egreso, props);

            // Assert
            result.Should().HaveCount(2);
            result.First(x => x.IdPropiedad == 1).MontoCalculado.Should().Be(500m);
            result.First(x => x.IdPropiedad == 2).MontoCalculado.Should().Be(9500m);
            result.Sum(x => x.MontoCalculado).Should().Be(10000m);
        }

        [Fact]
        public void CalculateDistribution_RoundingCase_ShouldHandlePeriodics()
        {
            // Arrange
            var egreso = new Egreso { IdEgreso = 1, MontoTotal = 1000m };
            // 3 units with 33.333...%
            var props = new List<Propiedad>
            {
                new Propiedad { IdPropiedad = 1, CodigoUnidad = "A", PorcentajeParticipacion = 33.333m, Activo = true },
                new Propiedad { IdPropiedad = 2, CodigoUnidad = "B", PorcentajeParticipacion = 33.333m, Activo = true },
                new Propiedad { IdPropiedad = 3, CodigoUnidad = "C", PorcentajeParticipacion = 33.334m, Activo = true } // Adjusted to sum 100
             };

            // Act
            var result = _service.CalculateDistribution(egreso, props);

            // Assert
            result.Sum(x => x.MontoCalculado).Should().Be(1000m);
            
            // Expected: 333.33, 333.33, 333.34
            var amounts = result.Select(x => x.MontoCalculado).OrderBy(x => x).ToList();
            amounts[0].Should().Be(333.33m);
            amounts[1].Should().Be(333.33m);
            amounts[2].Should().Be(333.34m);
        }

        [Fact]
        public void CalculateDistribution_InvalidSum_ShouldThrowException()
        {
            // Arrange
            var egreso = new Egreso { IdEgreso = 1, MontoTotal = 1000m };
            var props = new List<Propiedad>
            {
                new Propiedad { IdPropiedad = 1, CodigoUnidad = "A", PorcentajeParticipacion = 50m, Activo = true }
             };

            // Act
            Action act = () => _service.CalculateDistribution(egreso, props);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*no es 100%*");
        }
    }
}
