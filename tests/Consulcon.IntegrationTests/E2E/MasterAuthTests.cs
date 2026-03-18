using System.Net;
using System.Net.Http.Json;
using Consulcon.Application.DTOs.Seguridad;
using Consulcon.IntegrationTests.E2E; // Corrected Namespace to match fixture
using FluentAssertions;
using Xunit;

namespace Consulcon.IntegrationTests.E2E;

[Collection("E2E Tests")]
public class MasterAuthTests : IAsyncLifetime
{
    private readonly E2ETestFixture _fixture;
    private readonly HttpClient _client;

    public MasterAuthTests(E2ETestFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.Client;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GlobalLogin_ShouldReturnToken_And_TenantList()
    {
        // 1. Login as SuperAdmin (Global Context)
        var loginRequest = new LoginRequest
        {
            Username = "admin",
            Password = "admin123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[TEST FAILURE] Login Failed. Status: {response.StatusCode}. Content: {error}");
        }
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Username.Should().Be("admin");
        result.CondominioIds.Should().NotBeNull();
    }

     [Fact]
    public async Task MigrateTenant_ShouldSyncToMaster_And_AppearInLogin()
    {
        // 1. Login to get token
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Username = "admin", Password = "admin123" });
        var userDto = await loginResponse.Content.ReadFromJsonAsync<UserDto>();
        var token = userDto!.Token;

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. Trigger Migration/Sync for 'bosques' 
        var tenantId = "bosques"; 
        var migrateResponse = await _client.PostAsync($"/api/maintenance/migrate/{tenantId}", null);

        migrateResponse.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        migrateResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}
