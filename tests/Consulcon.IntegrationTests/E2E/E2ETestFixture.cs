using System.Net.Http.Headers;
using System.Net.Http.Json;
using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Consulcon.IntegrationTests.E2E;

/// <summary>
/// Fixture that creates a real test database, sets up test data, and cleans up after all tests run.
/// Use this for E2E tests that need a real MySQL database.
/// </summary>
public class E2ETestFixture : IAsyncLifetime
{
    private readonly string _testDbName;
    private readonly IConfiguration _configuration;
    
    public WebApplicationFactory<Program> Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;
    public string TestTenantId => _testDbName.Replace("db_condominio_", "");
    public string? AuthToken { get; private set; }
    public int TestPersonaId { get; private set; }
    public int TestCondominioId { get; private set; }
    public int TestUsuarioId { get; private set; }
    public int TestRolId { get; private set; }
    public int TestManzanoId { get; private set; }
    public int TestPropiedadId { get; private set; }
    public int TestServicioId { get; private set; }
    public int TestBancoId { get; private set; }
    public int TestCuentaId { get; private set; }
    public int TestContratoId { get; private set; }
    public int TestRecursoId { get; private set; }

    public E2ETestFixture()
    {
        // Generate unique test database name to avoid conflicts
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        _testDbName = $"db_condominio_test_{timestamp}";
        
        // Build configuration for test environment
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DB_HOST"] = "localhost",
                ["DB_PORT"] = "3310",
                ["DB_USER"] = "root",
                ["DB_PASSWORD"] = "root",
                ["UseInMemoryDatabase"] = "false"
            })
            .Build();
    }

    public async Task InitializeAsync()
    {
        // 1. Create the test database
        await CreateTestDatabaseAsync();
        
        // 2. Initialize schema using EF Core
        await InitializeSchemaAsync();
        
        // 2.1 Initialize Master DB Schema & Seed
        await InitializeMasterSchemaAsync();
        
        // 3. Create test data (Persona, Rol, Usuario, Condominio)
        await SeedTestDataAsync();
        
        // 4. Create Client (either InMemory or External)
        var externalUrl = Environment.GetEnvironmentVariable("TEST_API_URL");
        
        if (!string.IsNullOrEmpty(externalUrl))
        {
            // Use external API (e.g., Docker container)
            Client = new HttpClient { BaseAddress = new Uri(externalUrl) };
            Console.WriteLine($"Running tests against external API: {externalUrl}");
        }
        else
        {
            // Use InMemory WebApplicationFactory
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["DB_HOST"] = "localhost",
                            ["DB_PORT"] = "3310",
                            ["DB_USER"] = "root",
                            ["DB_PASSWORD"] = "root",
                            ["UseInMemoryDatabase"] = "false"
                        });
                    });
                });
            
            Client = Factory.CreateClient();
        }
        
        // 5. Authenticate and get token
        await AuthenticateAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up: Delete the test database
        Client?.Dispose();
        Factory?.Dispose();
        
        await DeleteTestDatabaseAsync();
    }

    private async Task CreateTestDatabaseAsync()
    {
        var connectionString = GetMasterConnectionString();
        
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{_testDbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
        await command.ExecuteNonQueryAsync();
    }

    private async Task InitializeSchemaAsync()
    {
        var connectionString = GetConnectionStringForDatabase(_testDbName);
        
        var optionsBuilder = new DbContextOptionsBuilder<ConsulconDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        await using var context = new ConsulconDbContext(optionsBuilder.Options);
        await context.Database.EnsureCreatedAsync();
    }

    private async Task InitializeMasterSchemaAsync()
    {
        try 
        {
            var connectionString = GetMasterConnectionString();
            var dbName = "db_consulcon_master";
            var masterConnString = connectionString.Replace(";", $";Database={dbName};");
            
            Console.WriteLine($"[FIXTURE] Initializing Master DB at {masterConnString.Replace("Password=", "Password=***")}");
            
            var optionsBuilder = new DbContextOptionsBuilder<ConsulconDbContext>();
            optionsBuilder.UseMySql(masterConnString, ServerVersion.AutoDetect(masterConnString));

            await using var context = new ConsulconDbContext(optionsBuilder.Options);
            await context.Database.EnsureCreatedAsync();
            
            // Seed Admin if not exists
            if (!await context.UsuariosMaster.AnyAsync(u => u.Username == "admin"))
            {
                Console.WriteLine("[FIXTURE] Seeding Admin user...");
                var adminUser = new Consulcon.Domain.Entities.Master.UsuarioMaster
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Email = "admin@consulcon.com",
                    EsSuperAdmin = true,
                    FechaCreacion = DateTime.UtcNow
                };
                context.UsuariosMaster.Add(adminUser);
                await context.SaveChangesAsync();
                Console.WriteLine("[FIXTURE] Admin user seeded.");
            }
            else
            {
                Console.WriteLine("[FIXTURE] Admin user already exists.");
            }
        }
        catch (Exception ex)
        {
             Console.WriteLine($"[FIXTURE ERROR] Master Init Failed: {ex.Message}");
             throw;
        }
    }

    private async Task SeedTestDataAsync()
    {
        var connectionString = GetConnectionStringForDatabase(_testDbName);
        
        var optionsBuilder = new DbContextOptionsBuilder<ConsulconDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        await using var context = new ConsulconDbContext(optionsBuilder.Options);
        
        // Create Rol
        var rol = new Consulcon.Domain.Entities.Seguridad.Rol { Nombre = "Admin" };
        context.Rols.Add(rol);
        await context.SaveChangesAsync();
        TestRolId = rol.IdRol;
        
        // Create Persona
        var persona = new Consulcon.Domain.Entities.General.Persona
        {
            NombreCompleto = "Test Admin",
            Ci = "TEST123",
            Sexo = "M",
            EsActivo = true
        };
        context.Personas.Add(persona);
        await context.SaveChangesAsync();
        TestPersonaId = persona.IdPersona;
        
        // Create Usuario with BCrypt hash
        var usuario = new Consulcon.Domain.Entities.Seguridad.Usuario
        {
            Username = "testadmin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("test123"),
            IdPersona = persona.IdPersona,
            IdRolPrincipal = rol.IdRol,
            EstaHabilitado = true,
            FechaCreacion = DateTime.UtcNow
        };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        TestUsuarioId = usuario.IdUsuario;
        
        // Create Condominio
        var condominio = new Consulcon.Domain.Entities.Inmuebles.Condominio
        {
            Codigo = "TEST",
            Nombre = "Condominio de Prueba",
            Direccion = "Calle Test 123",
            SuperficieTotalM2 = 1000,
            IdAdminPersona = persona.IdPersona
        };
        context.Condominios.Add(condominio);
        await context.SaveChangesAsync();
        TestCondominioId = condominio.IdCondominio;
        
        // Create Manzano
        var manzano = new Consulcon.Domain.Entities.Inmuebles.Manzano
        {
            IdCondominio = condominio.IdCondominio,
            Codigo = "M1",
            Nombre = "Manzano Test"
        };
        context.Manzanos.Add(manzano);
        await context.SaveChangesAsync();
        TestManzanoId = manzano.IdManzano;

        // Create Propiedad (for Contractual tests)
        var propiedad = new Consulcon.Domain.Entities.Inmuebles.Propiedad
        {
            IdManzano = manzano.IdManzano,
            CodigoUnidad = "U-SEED",
            NombreFuncional = "Propiedad Seed",
            SuperficieM2 = 100,
            Tipo = "Departamento"
        };
        context.Propiedads.Add(propiedad);
        await context.SaveChangesAsync();
        TestPropiedadId = propiedad.IdPropiedad;

        // Create Contrato (for Cobranza tests)
        var contrato = new Consulcon.Domain.Entities.Contratos.Contrato
        {
            IdPropiedad = propiedad.IdPropiedad,
            FechaFirma = DateOnly.FromDateTime(DateTime.Now),
            FechaInicio = DateOnly.FromDateTime(DateTime.Now),
            FechaFin = DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
            MontoExpensaPactada = 500,
            IdUsuarioCreador = usuario.IdUsuario
            // Activo property removed as it doesn't exist
        };
        context.Contratos.Add(contrato);
        await context.SaveChangesAsync();
        TestContratoId = contrato.IdContrato;

        // Create Servicio (for Deuda tests)
        var servicio = new Consulcon.Domain.Entities.Contratos.CatalogoServicio
        {
            Nombre = "Servicio Seed",
            CostoBase = 100,
            EsRecurrente = true,
            Activo = true
        };
        context.CatalogoServicios.Add(servicio);
        await context.SaveChangesAsync();
        TestServicioId = servicio.IdServicio;
        
        // Create PlanCuenta (for Contabilidad tests)
        var cuenta = new Consulcon.Domain.Entities.Contabilidad.PlanCuenta
        {
            CodigoCuenta = "1.1.01",
            Nombre = "Caja",
            EsImputable = true,
            NivelJerarquia = 1
        };
        context.PlanCuentas.Add(cuenta);
        await context.SaveChangesAsync();
        TestCuentaId = cuenta.IdCuenta;

        // Create Banco (for Pago tests)
        var banco = new Consulcon.Domain.Entities.General.Banco
        {
            NombreEntidad = "Banco Seed",
            NumeroCuenta = "12345",
            Moneda = "BOB",
            IdCuentaContableAsociada = cuenta.IdCuenta
        };
        context.Bancos.Add(banco);
        await context.SaveChangesAsync();
        TestBancoId = banco.IdBanco;

        // Create RecursoComun (for Eventos tests)
        var recurso = new Consulcon.Domain.Entities.Reservas.RecursoComun
        {
            IdCondominio = condominio.IdCondominio,
            Nombre = "Churrasquera Test",
            CostoReserva = 100,
            CostoGarantia = 50,
            ColorCalendario = "#FFFFFF"
        };
        context.RecursoComuns.Add(recurso);
        await context.SaveChangesAsync();
        TestRecursoId = recurso.IdRecurso;
    }

    private async Task AuthenticateAsync()
    {
       Client.DefaultRequestHeaders.Add("X-Condominio-Id", "1");
        
        var loginRequest = new Consulcon.Application.DTOs.Seguridad.LoginRequest
        {
            Username = "admin",
            Password = "admin123"
        };
        
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var jsonString = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine("\n================================================");
        Console.WriteLine($"[FIXTURE DEBUG] HTTP STATUS: {response.StatusCode}");
        Console.WriteLine($"[FIXTURE DEBUG] JSON DEL LOGIN: {jsonString}");
        Console.WriteLine("================================================\n");
        
        if (response.IsSuccessStatusCode)
        {
            var jsonNode = System.Text.Json.Nodes.JsonNode.Parse(jsonString);
            
            var tokenString = jsonNode?["token"]?.ToString() 
                           ?? jsonNode?["Token"]?.ToString()
                           ?? jsonNode?["data"]?["token"]?.ToString() 
                           ?? jsonNode?["Data"]?["Token"]?.ToString();

            if (!string.IsNullOrEmpty(tokenString))
            {
                AuthToken = tokenString;
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                Console.WriteLine($"[FIXTURE] ¡EXITO! Auth Token asignado correctamente.");
            }
            else
            {
                Console.WriteLine($"[FIXTURE ERROR] No se pudo extraer la propiedad 'token' del JSON.");
            }
        }
    }

    private async Task DeleteTestDatabaseAsync()
    {
        try
        {
            var connectionString = GetMasterConnectionString();
            
            await using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = $"DROP DATABASE IF EXISTS `{_testDbName}`;";
            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            // Ignore cleanup errors - database might already be deleted
        }
    }

    private string GetMasterConnectionString()
    {
        var dbHost = _configuration["DB_HOST"];
        var dbPort = _configuration["DB_PORT"] ?? "3306";
        var dbUser = _configuration["DB_USER"];
        var dbPassword = _configuration["DB_PASSWORD"];
        return $"Server={dbHost};Port={dbPort};User={dbUser};Password={dbPassword};";
    }

    private string GetConnectionStringForDatabase(string databaseName)
    {
        var dbHost = _configuration["DB_HOST"];
        var dbPort = _configuration["DB_PORT"] ?? "3306";
        var dbUser = _configuration["DB_USER"];
        var dbPassword = _configuration["DB_PASSWORD"];
        return $"Server={dbHost};Port={dbPort};Database={databaseName};User={dbUser};Password={dbPassword};";
    }

    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public UserDto? Data { get; set; }
    }
}
