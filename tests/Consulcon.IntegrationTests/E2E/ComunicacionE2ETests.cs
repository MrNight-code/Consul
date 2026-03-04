using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Tests E2E para módulo de Comunicación (Blog, Comunicados).
/// </summary>
[Collection("E2E Tests")]
public class ComunicacionE2ETests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    [Fact]
    public async Task GetComunicados_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync($"/api/comunicacion/condominio/{_fixture.TestCondominioId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateComunicado_WithValidData_ReturnsCreated()
    {
        var comunicadoDto = new
        {
            IdCondominio = _fixture.TestCondominioId,
            Titulo = "Aviso de Mantenimiento Test",
            ContenidoHtml = "<p>El ascensor estará en mantenimiento el lunes.</p>",
            UrlImagen = "img/aviso_test.png"
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/comunicacion", comunicadoDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }
}
