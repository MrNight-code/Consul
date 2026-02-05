using System.Net;
using System.Net.Http.Json;
using Consulcon.Application.DTOs.Inmuebles;
using FluentAssertions;

namespace Consulcon.IntegrationTests.Controllers;

public class CondominioControllerTests : IntegrationTest
{
    public CondominioControllerTests(ConsulconWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateCondominio_ShouldReturnCreated_WhenDataIsValid()
    {
        var personaId = await AuthenticateAsync();

        var request = new CondominioDto 
        {
            Nombre = "Condominio Test",
            Direccion = "Calle 123",
            Logo = "logo.png",
            ConfigDiaCobro = "5",
            IdAdminPersona = personaId
        };
        
        var response = await _client.PostAsJsonAsync("/api/Condominio", request);

        if (response.StatusCode != HttpStatusCode.Created)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with {response.StatusCode}. Response: {content}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<CondominioDto>();
        result.Should().NotBeNull();
        result!.Id.Should().BeGreaterThan(0);
        result.Nombre.Should().Be("Condominio Test");
    }

    [Fact]
    public async Task GetAll_ShouldReturnList_WhenCondominiosExist()
    {
        var personaId = await AuthenticateAsync();

        var responseCreate1 = await _client.PostAsJsonAsync("/api/Condominio", new CondominioDto { Nombre = "C1", Direccion = "D1", IdAdminPersona = personaId });
        if (!responseCreate1.IsSuccessStatusCode)
             throw new Exception($"Setup Create failed: {await responseCreate1.Content.ReadAsStringAsync()}");

        var responseCreate2 = await _client.PostAsJsonAsync("/api/Condominio", new CondominioDto { Nombre = "C2", Direccion = "D2", IdAdminPersona = personaId });
        if (!responseCreate2.IsSuccessStatusCode)
             throw new Exception($"Setup Create 2 failed: {await responseCreate2.Content.ReadAsStringAsync()}");

        var response = await _client.GetAsync("/api/Condominio");

       if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Get failed with {response.StatusCode}. Response: {content}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CondominioDto>>();
        
        result.Should().NotBeNull();
        result.Count().Should().BeGreaterThanOrEqualTo(2);
    }
}
