# Guía de Testing E2E

Esta guía describe cómo ejecutar y extender los tests de integración End-to-End (E2E) del proyecto. Estos tests verifican el funcionamiento completo de la API interactuando con una base de datos MySQL real.

## 1. Prerrequisitos

Para ejecutar los tests E2E, necesitas:

- **Docker** instalado y corriendo.
- Una instancia de **MySQL** ejecutándose en el puerto **3310** (configuración por defecto de `docker-compose.yml`).

## 2. Cómo Ejecutar los Tests

### Ejecutar Solo Tests E2E

```bash
dotnet test tests/Consulcon.IntegrationTests --filter "FullyQualifiedName~E2E"
```

### Ejecutar Todos los Tests (E2E + Unitarios/Integration)

```bash
dotnet test tests/Consulcon.IntegrationTests
```

### Ejecutar Tests contra Docker (API Real)

Es posible ejecutar los tests E2E contra la API corriendo en Docker (`http://localhost:5000`) en lugar de usar la API en memoria. Esto es útil para validar la imagen de Docker final.

1. Asegúrate que `docker-compose up` esté corriendo.
2. Ejecuta los tests definiendo `TEST_API_URL`:

**PowerShell:**

```powershell
$env:TEST_API_URL="http://localhost:5000"; dotnet test tests/Consulcon.IntegrationTests --filter "FullyQualifiedName~E2E"
```

**Bash:**

```bash
TEST_API_URL="http://localhost:5000" dotnet test tests/Consulcon.IntegrationTests --filter "FullyQualifiedName~E2E"
```

## 3. Arquitectura de Tests E2E

Los tests utilizan un **Fixture Compartido** (`E2ETestFixture.cs`) que gestiona el ciclo de vida del entorno de prueba.

### Ciclo de Vida Automático

1. **Inicio Global**: Al iniciar la suite de tests, el Fixture:

   - Genera un nombre de base de datos único (`db_condominio_test_YYYYMMDDHHmmss`).
   - Crea la base de datos en MySQL.
   - Aplica las migraciones (crea tablas).
   - Inserta datos semilla (Roles, Usuario Admin, Condominio, Propiedad, Servicios, etc.).
   - Autentica al usuario Admin y obtiene un token JWT.

2. **Ejecución de Tests**: Todos los tests comparten la misma base de datos y token para máxima velocidad.

3. **Limpieza (Teardown)**: Al finalizar todos los tests, el Fixture **elimina la base de datos creada**, dejando el entorno limpio.

## 4. Escribiendo Nuevos Tests

Hereda de `IClassFixture<E2ETestFixture>` y usa el cliente HTTP pre-configurado.

```csharp
[Collection("E2E Tests")]
public class MiNuevoTest(E2ETestFixture fixture) : IClassFixture<E2ETestFixture>
{
    private readonly E2ETestFixture _fixture = fixture;

    [Fact]
    public async Task GetEndpoint_ReturnsOk()
    {
        // El cliente ya tiene el BaseAddress y el Token JWT configurados
        var response = await _fixture.Client.GetAsync("/api/MiEndpoint");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

### Uso de IDs Dinámicos

Dado que la base de datos se crea desde cero, **NO uses IDs hardcodeados** (ej: `1`). Usa las propiedades expuestas por el fixture:

- `_fixture.TestCondominioId`
- `_fixture.TestUsuarioId`
- `_fixture.TestPropiedadId`
- `_fixture.TestManzanoId`
- `_fixture.TestServicioId`
- `_fixture.TestContratoId`
- `_fixture.TestBancoId`
- `_fixture.TestCuentaId`
- `_fixture.TestRecursoId`

Ejemplo:

```csharp
var contratoDto = new {
    IdPropiedad = _fixture.TestPropiedadId, // ✅ Correcto
    // IdPropiedad = 1 // ❌ Incorrecto (puede fallar si cambian los seeds)
};
```

## 5. Solución de ProblemasCommon Issues

- **Error de Conexión**: Verifica que Docker esté corriendo y el puerto 3310 esté expuesto.
- **Foreign Key Constraint Fails**: Asegúrate de usar los IDs dinámicos del fixture (`_fixture.TestXId`) en lugar de IDs fijos.
