using System.Net;
using System.Net.Http.Json;
using Consulcon.Application.DTOs.Seguridad;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Consulcon.IntegrationTests.Controllers;

public class AuthControllerTests : IntegrationTest
{
    public AuthControllerTests(ConsulconWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Add Persona first as Usuario depends on it
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Consulcon.Infrastructure.Persistence.ConsulconDbContext>();
            
            var persona = new Consulcon.Domain.Entities.General.Persona
            {
                NombreCompleto = "Admin User",
                Ci = "1234567", 
                EsActivo = true
            };
            db.Personas.Add(persona);
            await db.SaveChangesAsync();

            db.Usuarios.Add(new Consulcon.Domain.Entities.Seguridad.Usuario
            {
                Username = "testadmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                EstaHabilitado = true,
                IdPersona = persona.IdPersona,
                FechaCreacion = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        var loginRequest = new LoginRequest
        {
            Username = "testadmin",
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result!.Data.Should().NotBeNull();
        result.Data.Username.Should().Be("testadmin");
        result.Data.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        var loginRequest = new LoginRequest
        {
            Username = "invalid",
            Password = "user"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
