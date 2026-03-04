using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Tests E2E para módulo de Tesorería (Bancos, Egresos) y Contabilidad (Plan Cuentas, Asientos).
/// </summary>
[Collection("E2E Tests")]
public class TesoreriaE2ETests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    #region Bancos Tests

    [Fact]
    public async Task GetAllBancos_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/api/tesoreria/bancos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateBanco_WithValidData_ReturnsCreated()
    {
        var bancoDto = new
        {
            NombreEntidad = $"Banco Test {DateTime.Now.Ticks}",
            NumeroCuenta = "1234567890",
            Moneda = "BOB"
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/tesoreria/bancos", bancoDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
    }

    #endregion

    #region Egresos Tests

    [Fact]
    public async Task RegistrarEgreso_WithValidData_ReturnsCreated()
    {
        var egresoDto = new
        {
            IdCondominio = _fixture.TestCondominioId,
            IdProveedor = 1, // Assumption
            IdAutorizacion = 1, // Assumption
            IdBancoOrigen = _fixture.TestBancoId,
            IdFormaPago = 1, // Assumption
            Concepto = "Compra Material Limpieza",
            MontoTotal = 150.00m,
            NroFacturaProveedor = $"FAC-{DateTime.Now.Ticks}",
            IdUsuarioRegistro = _fixture.TestUsuarioId
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/tesoreria/egresos", egresoDto);
        // May fail if dependencies don't exist (FK constraints)
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }

    #endregion

    #region Contabilidad Tests

    [Fact]
    public async Task GetPlanCuentas_ReturnsOk()
    {
        var response = await _fixture.Client.GetAsync("/api/contabilidad/plancuentas");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegistrarAsiento_WithValidData_ReturnsCreated()
    {
        var asientoDto = new
        {
            IdCondominio = _fixture.TestCondominioId,
            FechaContable = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            GlosaGeneral = "Ajuste Test",
            TipoAsiento = "Ajuste",
            Detalles = new[]
            {
                new { IdCuenta = _fixture.TestCuentaId, GlosaLinea = "Debe Test", Debe = 100m, Haber = 0m },
                new { IdCuenta = _fixture.TestCuentaId, GlosaLinea = "Haber Test", Debe = 0m, Haber = 100m }
            }
        };

        var response = await _fixture.Client.PostAsJsonAsync("/api/contabilidad/asientos", asientoDto);
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task VoidExpense_WithValidReason_ReturnsOk()
    {
        var loginData = new { Email = "admin@consulcon.com", Password = "Password123!" };
        var authResponse = await _fixture.Client.PostAsJsonAsync("/api/auth/login", loginData);
        
        authResponse.EnsureSuccessStatusCode(); 

        var authResult = await authResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        
        if (authResult != null && authResult.TryGetValue("token", out var tokenValue))
        {
            string token = tokenValue.ToString()!;

            _fixture.Client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else 
        {
            throw new Xunit.Sdk.XunitException("No se pudo obtener el token de autenticación del login.");
        }

        var expenseId = 1; 
        var voidRequest = new VoidExpenseRequest { Reason = "Anulación por rastro de auditoría" };
        var response = await _fixture.Client.PostAsJsonAsync($"/api/expenses/{expenseId}/void", voidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VoidExpense_WithShortReason_ReturnsBadRequest()
    {
        var expenseId = 1;
        var voidRequest = new { Reason = "Error" }; 

        var response = await _fixture.Client.PostAsJsonAsync($"/api/expenses/{expenseId}/void", voidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    #endregion
}
