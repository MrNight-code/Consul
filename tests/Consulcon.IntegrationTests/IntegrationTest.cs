using Consulcon.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Consulcon.IntegrationTests;

public abstract class IntegrationTest : IClassFixture<ConsulconWebApplicationFactory>
{
    protected readonly HttpClient _client;
    protected readonly ConsulconWebApplicationFactory _factory;

    protected IntegrationTest(ConsulconWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Condominio-Id", "1");
    }


    protected async Task<int> AuthenticateAsync()
    {
        int personaId = 0;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<Consulcon.Infrastructure.Persistence.ConsulconDbContext>();
            
            var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Username == "integrationuser");
            if (user == null)
            {
                var persona = new Consulcon.Domain.Entities.General.Persona
                {
                    NombreCompleto = "Integration User",
                    Ci = "999999", 
                    EsActivo = true
                };
                db.Personas.Add(persona);
                await db.SaveChangesAsync();

                user = new Consulcon.Domain.Entities.Seguridad.Usuario
                {
                    Username = "integrationuser",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    IdRolPrincipal = 1,
                    EstaHabilitado = true,
                    IdPersona = persona.IdPersona,
                    FechaCreacion = DateTime.UtcNow
                };
                db.Usuarios.Add(user);
                await db.SaveChangesAsync();
            }
            personaId = user.IdPersona;
        }

        var response = await _client.PostAsJsonAsync("/api/auth/login", new Consulcon.Application.DTOs.Seguridad.LoginRequest
        {
            Username = "integrationuser",
            Password = "password123"
        });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result?.Data != null && !string.IsNullOrEmpty(result.Data.Token))
            {
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Data.Token);
                return personaId;
            }
        }
        return 0;
    }

    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public Consulcon.Application.DTOs.Seguridad.UserDto Data { get; set; } = default!;
    }
}
