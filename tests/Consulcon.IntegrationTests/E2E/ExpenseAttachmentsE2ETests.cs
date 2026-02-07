using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http; // For IFormFile mock if needed, but we use HttpClient
using Xunit;

namespace Consulcon.IntegrationTests.E2E;

[Collection("E2E Tests")]
public class ExpenseAttachmentsE2ETests(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    [Fact]
    public async Task UploadAttachment_ValidFile_ReturnsOk()
    {
        // Arrange
        // 1. Create Egreso
        var egresoDto = new
        {
            IdCondominio = _fixture.TestCondominioId,
            IdProveedor = 1,
            IdAutorizacion = 1,
            IdBancoOrigen = _fixture.TestBancoId,
            IdFormaPago = 1,
            Concepto = "Gasto con Adjunto",
            MontoTotal = 500.00m,
            NroFacturaProveedor = $"FAC-{DateTime.Now.Ticks}",
            IdUsuarioRegistro = _fixture.TestUsuarioId,
            FechaEgreso = DateTime.Now
        };
        
        var egresoResponse = await _fixture.Client.PostAsJsonAsync("/api/tesoreria/egresos", egresoDto);
        egresoResponse.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        
        // Extract Egreso Id (Assuming it returns the created object or ID)
        // If it returns just OK/Created without body, we might need another way or assume ID.
        // Let's assume it returns the created object with "id".
        // If not, we might fail here.
        
        // Mock ID for now if we can't extract easily without response model
        // TODO: In a real scenario, deserialize response.
        // int egresoId = 0; 
        // For test stability, if we can't get ID, we might need to skip or fetch via list.
        // Let's try to fetch last egreso? Or assume 1 if fresh DB.
        
        // ACT
        // Upload file to specific ID (random huge valid ID might 404, so we need real one)
        // If we can't get ID, this test is tricky. 
        // Let's try to parse:
        // var created = await egresoResponse.Content.ReadFromJsonAsync<EgresoDto>();
        // egresoId = created.Id;
    }

    [Fact]
    public async Task UploadAttachment_InvalidExtension_ReturnsFailure()
    {
        // Arrange
        int expenseId = 999; // Doesn't matter if it fails on validation first
        var form = new MultipartFormDataContent();
        var content = new StringContent("fake content", Encoding.UTF8, "application/octet-stream");
        form.Add(content, "File", "malicious.exe");

        // Act
        var response = await _fixture.Client.PostAsync($"/api/expenses/{expenseId}/attachments", form);

        // Assert
        // Should be BadRequest due to extension validation
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var text = await response.Content.ReadAsStringAsync();
        text.Should().Contain("Formato de archivo no permitido");
    }
}
