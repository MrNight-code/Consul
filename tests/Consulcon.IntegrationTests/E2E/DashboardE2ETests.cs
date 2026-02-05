using System.Net;
using System.Net.Http.Json;
using Consulcon.Application.DTOs.Dashboard;
using FluentAssertions;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Tests E2E para Dashboard - Contadores agregados del condominio.
/// </summary>
[Collection("E2E Tests")]
public class DashboardE2ETests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    [Fact]
    public async Task GetContadores_WithValidCondominioId_ReturnsOk()
    {
        // Arrange
        var condominioId = _fixture.TestCondominioId;

        // Act
        var response = await _fixture.Client.GetAsync($"/api/Dashboard/{condominioId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<DashboardCountersDto>();
        result.Should().NotBeNull();
        result!.TotalUnidades.Should().BeGreaterThanOrEqualTo(0);
        result.UnidadesEnMora.Should().BeLessThanOrEqualTo(result.TotalUnidades);
        result.TotalCobradoMesActual.Should().BeGreaterThanOrEqualTo(0);
        result.PorcentajeCobranza.Should().BeGreaterThanOrEqualTo(0).And.BeLessThanOrEqualTo(100);
        result.CondominioNombre.Should().NotBeNullOrEmpty();
        result.UltimaActualizacion.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetContadores_WithInvalidCondominioId_ReturnsNotFound()
    {
        // Arrange
        var invalidCondominioId = 99999;

        // Act
        var response = await _fixture.Client.GetAsync($"/api/Dashboard/{invalidCondominioId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RefrescarContadores_WithValidCondominioId_ReturnsOk()
    {
        // Arrange
        var condominioId = _fixture.TestCondominioId;

        // Act
        var response = await _fixture.Client.PostAsync($"/api/Dashboard/{condominioId}/refrescar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<DashboardCountersDto>();
        result.Should().NotBeNull();
        result!.CondominioNombre.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefrescarContadores_WithInvalidCondominioId_ReturnsNotFound()
    {
        // Arrange
        var invalidCondominioId = 99999;

        // Act
        var response = await _fixture.Client.PostAsync($"/api/Dashboard/{invalidCondominioId}/refrescar", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetContadores_UnidadesEnMoraShouldNotExceedTotalUnidades()
    {
        // Arrange
        var condominioId = _fixture.TestCondominioId;

        // Act
        var response = await _fixture.Client.GetAsync($"/api/Dashboard/{condominioId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<DashboardCountersDto>();
        result.Should().NotBeNull();
        result!.UnidadesEnMora.Should().BeLessThanOrEqualTo(result.TotalUnidades);
    }

    [Fact]
    public async Task PorcentajeCobranzaShouldBeValidRange()
    {
        // Arrange
        var condominioId = _fixture.TestCondominioId;

        // Act
        var response = await _fixture.Client.GetAsync($"/api/Dashboard/{condominioId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<DashboardCountersDto>();
        result.Should().NotBeNull();
        result!.PorcentajeCobranza.Should().BeGreaterThanOrEqualTo(0).And.BeLessThanOrEqualTo(100);
    }
}
