using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Tests E2E para módulo de Servicios (CatalogoServicio).
/// </summary>
[Collection("E2E Tests")]
public class ServiciosE2ETests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    [Fact]
    public async Task GetAllServicios_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/api/CatalogoServicio");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateServicio_WithValidData_ReturnsCreated()
    {
        var servicioDto = new
        {
            Nombre = $"Expensa Test {DateTime.Now.Ticks}",
            CostoBase = 500.00m,
            EsRecurrente = true,
            Activo = true
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/CatalogoServicio", servicioDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateServicio_Agua_ReturnsCreated()
    {
        var servicioDto = new
        {
            Nombre = "Agua Potable",
            CostoBase = 0.00m,
            EsRecurrente = true,
            Activo = true
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/CatalogoServicio", servicioDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }
}
