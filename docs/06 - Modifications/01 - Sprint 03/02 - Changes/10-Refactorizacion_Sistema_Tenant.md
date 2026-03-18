# 5. Refactorización de Sistema de Identificación de Condominios - Cambios

**Sprint:** 3
**Tipo:** Change
**Fecha:** 07/02/2026
**Módulo:** Core / Multi-tenancy

## Descripción

Refactorización completa del sistema de identificación de condominios para usar IDs numéricos en lugar de nombres sanitizados como identificadores. Este cambio mejora la robustez, simplifica la lógica de resolución de tenant, y agrega validación de errores cuando un condominio no existe.

## Resumen de Cambios

### Antes (Problemático)

- Header: `X-Tenant-Id: bosques_colina` (nombre sanitizado)
- DB Name: `db_condominio_bosques_colina`
- Problema: Si el nombre contenía "condominio", el DB name se duplicaba

### Después (Actual)

- Header: `X-Condominio-Id: 1` (ID numérico)
- DB Name: Resuelto desde `CondominiosMaster.TenantId`
- Ventaja: IDs simples, resolución desde Master DB, validación de existencia

## Cambios en API

### 1. `CurrentTenantService`

Ubicación: `src/Consulcon.API/Services/CurrentTenantService.cs`

- **Modificación**: Cambio de header y adición de validación de errores.

  ```csharp
  // Nuevo: Lee X-Condominio-Id en lugar de X-Tenant-Id
  context.Request.Headers.TryGetValue("X-Condominio-Id", out var condominioIdHeader)

  // Nuevo: Propiedades para detectar errores de resolución
  public bool TenantResolutionFailed { get; }
  public string? TenantResolutionError { get; }
  ```

### 2. `TenantValidationMiddleware` [NUEVO]

Ubicación: `src/Consulcon.API/Middleware/TenantValidationMiddleware.cs`

- **Nueva funcionalidad**: Valida que si se proporciona `X-Condominio-Id`, el condominio debe existir.
- **Comportamiento**: Retorna 400 Bad Request con mensaje descriptivo si el ID no existe.
  ```json
  {
    "isSuccess": false,
    "errorCode": "ERR-TENANT-404",
    "message": "Condominio 99 no existe o fue eliminado."
  }
  ```

### 3. `ICurrentTenantService`

Ubicación: `src/Consulcon.Domain/Interfaces/ICurrentTenantService.cs`

- **Modificación**: Agregadas propiedades para manejo de errores.
  ```csharp
  bool TenantResolutionFailed { get; }
  string? TenantResolutionError { get; }
  ```

## Cambios en Usuario Endpoints

### `UsuarioService` - Ahora usa Master DB

Ubicación: `src/Consulcon.Application/Services/Seguridad/UsuarioService.cs`

- **Antes**: Usaba `IRepository<Usuario>` (entidad tenant, requería `X-Condominio-Id`)
- **Después**: Usa `IRepository<UsuarioMaster>` (entidad master, NO requiere header)

### `CreateUserDto` - Simplificado

Ubicación: `src/Consulcon.Application/DTOs/Seguridad/CreateUserDto.cs`

```csharp
// Antes
public class CreateUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public int IdPersona { get; set; }      // FK a tenant
    public int? IdRolPrincipal { get; set; } // FK a tenant
}

// Después
public class CreateUserDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
}
```

## Endpoints Afectados

| Endpoint                                       | Antes                      | Después                          |
| ---------------------------------------------- | -------------------------- | -------------------------------- |
| `GET/POST /api/Usuario`                        | Requería `X-Condominio-Id` | NO requiere header (Master DB)   |
| `GET/POST /api/Condominio`                     | Requería `X-Tenant-Id`     | NO requiere header (CRUD Master) |
| Endpoints de tenant (Manzano, Propiedad, etc.) | `X-Tenant-Id: nombre`      | `X-Condominio-Id: numero`        |

## Cambios en Postman

Todos los endpoints de la carpeta `Entities/Usuario` y `Entities/Condominio` fueron actualizados:

- Eliminado header `X-Tenant-Id`
- Usuario: No usa headers de condominio (opera en Master)
- Condominio: No usa headers de condominio (opera en Master)
- Otros endpoints tenant: Usan `X-Condominio-Id` con valor numérico

## Impacto

- **Breaking Change**: El header `X-Tenant-Id` ya no es soportado, usar `X-Condominio-Id`
- **Mejora de Seguridad**: IDs inválidos ahora retornan error 400 en lugar de caer al Master DB
- **Simplificación**: Usuarios se crean sin necesidad de contexto de condominio
- **Postman**: Actualizada la colección con los nuevos headers
