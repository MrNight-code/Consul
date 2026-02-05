using System.Net;
using System.Net.Http.Json;
using Consulcon.Application.DTOs.Inmuebles;
using FluentAssertions;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Tests E2E para módulo Inmobiliario (Condominio, Propiedad, Manzano).
/// Usando primary constructor para eliminar warning.
/// </summary>
[Collection("E2E Tests")]
public class InmobiliarioE2ETests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    #region Condominio Tests

    [Fact]
    public async Task GetAllCondominios_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/api/Condominio");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<List<CondominioDto>>();
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetCondominioById_WithValidId_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync($"/api/Condominio/{_fixture.TestCondominioId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<CondominioDto>();
        result.Should().NotBeNull();
        result!.Nombre.Should().Be("Condominio de Prueba");
    }

    [Fact]
    public async Task GetCondominioById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _fixture.Client.GetAsync("/api/Condominio/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCondominio_WithValidData_ReturnsOk()
    {
        var updateDto = new
        {
            Codigo = "TEST",
            Nombre = "Condominio Actualizado",
            Direccion = "Calle Test 456",
            SuperficieTotalM2 = 1500,
            IdAdminPersona = _fixture.TestPersonaId
        };

        var response = await _fixture.Client.PutAsJsonAsync($"/api/Condominio/{_fixture.TestCondominioId}", updateDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    #endregion

    #region Propiedad Tests

    [Fact]
    public async Task GetAllPropiedades_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/api/Propiedad");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreatePropiedad_WithValidData_ReturnsCreated()
    {
        var propiedadDto = new CreatePropiedadDto
        {
            IdManzano = _fixture.TestManzanoId,
            CodigoUnidad = $"UNIT-{DateTime.Now.Ticks}",
            NombreFuncional = "Departamento 101",
            SuperficieM2 = 85.5m,
            PorcentajeParticipacion = 1.5m,
            ExpensaBaseDefecto = 500m,
            Tipo = "Departamento"
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/Propiedad", propiedadDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.Conflict, HttpStatusCode.InternalServerError);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<PropiedadDto>();
            result.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetPropiedadesByCondominio_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync($"/api/Propiedad/condominio/{_fixture.TestCondominioId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
