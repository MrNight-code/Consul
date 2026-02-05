using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Tests E2E para módulo de Administración (Personas, Usuarios).
/// </summary>
[Collection("E2E Tests")]
public class AdministracionE2ETests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    #region Persona Tests

    [Fact]
    public async Task GetAllPersonas_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/api/Persona");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreatePersona_WithValidData_ReturnsCreated()
    {
        var personaDto = new
        {
            NombreCompleto = "Juan Perez Test",
            Ci = "7654321",
            FechaNacimiento = "1990-01-01",
            Sexo = "M",
            EstadoCivil = "Soltero",
            EsActivo = true
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/Persona", personaDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPersonaById_WithValidId_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync($"/api/Persona/{_fixture.TestPersonaId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePersona_WithValidData_ReturnsOk()
    {
        var updateDto = new
        {
            NombreCompleto = "Persona Actualizada",
            Ci = "TEST123",
            Sexo = "M",
            EsActivo = true
        };

        var response = await _fixture.Client.PutAsJsonAsync($"/api/Persona/{_fixture.TestPersonaId}", updateDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    #endregion

    #region Usuario Tests

    [Fact]
    public async Task GetAllUsuarios_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/api/Usuario");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUsuarioById_WithValidId_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/api/Usuario/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
