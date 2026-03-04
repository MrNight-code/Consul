using System.Net;
using System.Net.Http.Json;
using Consulcon.Application.DTOs.Seguridad;
using FluentAssertions;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Tests de autenticación usando base de datos MySQL real.
/// </summary>
[Collection("E2E Tests")]
public class AuthE2ETests : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture;
    private readonly HttpClient _client;

    public AuthE2ETests(E2ETestFixture fixture)
    {
        _fixture = fixture;
        // Use the shared client from fixture which handles both InMemory and External configuration
        _client = fixture.Client;
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "testadmin",
            Password = "test123"
        };

        // Act
        // _client already has X-Tenant-Id header set in Fixture.InitializeAsync
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<E2ETestFixture.LoginResponse>();
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.Username.Should().Be("testadmin");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "invalid",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithWrongTenantId_ReturnsUnauthorized()
    {
        // Arrange
        HttpClient client;
        if (_fixture.Factory != null)
        {
             client = _fixture.Factory.CreateClient();
        }
        else
        {
             // Running against external API
             client = new HttpClient { BaseAddress = _fixture.Client.BaseAddress };
        }
        
        client.DefaultRequestHeaders.Add("X-Condominio-Id", "999"); // nonexistent_condominio
        
        var loginRequest = new LoginRequest
        {
            Username = "testadmin",
            Password = "test123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        // Should fail because database doesn't exist
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.InternalServerError);
    }
}
