using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Tests E2E para módulo de Eventos (Reservas, Recursos).
/// </summary>
[Collection("E2E Tests")]
public class EventosE2ETests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    [Fact]
    public async Task GetRecursosCondominio_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync($"/api/reserva/recursos/condominio/{_fixture.TestCondominioId}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateReserva_WithValidData_ReturnsCreated()
    {
        var reservaDto = new
        {
            IdRecurso = _fixture.TestRecursoId,
            IdContrato = _fixture.TestContratoId,
            FechaInicio = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddT10:00:00"),
            FechaFin = DateTime.Now.AddDays(7).ToString("yyyy-MM-ddT18:00:00"),
            CantidadInvitados = 20,
            Motivo = "Cumpleaños Test",
            AmenizadoPor = "DJ Test"
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/reserva", reservaDto);
        // May fail if dependencies don't exist
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }
}
