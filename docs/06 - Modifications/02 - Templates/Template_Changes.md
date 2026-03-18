# [Number]. [Feature Name] - Cambios

**Sprint:** [Sprint Number]
**Tipo:** Change
**Fecha:** [DD/MM/YYYY]
**Módulo:** [Module Name]

## Descripción

[Description of the change, refactoring, or modification].

## Cambios en Infraestructura

### 1. `ConsulconDbContext`

Ubicación: `src/Consulcon.Infrastructure/Persistence/ConsulconDbContext.cs`

- **Modificación**: [Description of what was added, e.g., new DbSets].
  ```csharp
  // Code example
  public virtual DbSet<Domain.Entities.[Folder].[Entity]> [DbSetName] { get; set; } = null!;
  ```

### 2. Dependency Injection

Ubicación: `src/Consulcon.Infrastructure/DependencyInjection.cs`

- **Modificación**: [Description of new service registration].
  ```csharp
  services.AddScoped<I[Name]Service, Services.[Name]Service>();
  ```

## Cambios en DTOs

### `[DtoName]` (Modificado)

| Campo         | Cambio                              | Descripción               |
| ------------- | ----------------------------------- | ------------------------- |
| `[FieldName]` | **[Eliminado/Agregado/Modificado]** | [Razonamiento del cambio] |

**Ubicación:** `src/Consulcon.Application/DTOs/[Folder]/[DtoName].cs`

## Endpoints Modificados

### 1. [Nombre del Endpoint]

| Propiedad             | Valor                                               |
| --------------------- | --------------------------------------------------- |
| **Método**            | `POST` / `PUT`                                      |
| **Ruta**              | `api/[ControllerName]/[Route]`                      |
| **Descripción**       | [Descripción del cambio en el endpoint]             |
| **Body / Parámetros** | [Nuevas reglas, parámetros, headers omitidos, etc.] |

## Cambios en Lógica de Negocio

- **Servicio Afectado**: `[ServiceName]`
- **Nuevos Cambios**: [Describe what changed in the logic].
- **Flujo de Datos**: [Explain any changes in how data moves or is stored].

## Impacto

- [List potential impacts on other parts of the system].
- [Mention any breaking changes or required migrations].

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02 - Postman/postman_collection.json`  
**Carpeta:** `[Entities Folder] > [EntityName]`

### Request 1: [Request Name] (Modificado)

| Campo                | Valor                                                                      |
| -------------------- | -------------------------------------------------------------------------- |
| **Nombre**           | [Request Name]                                                             |
| **Método**           | `[HTTP Method]`                                                            |
| **URL**              | `{{baseUrl}}/api/[Route]`                                                  |
| **Headers**          | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}` |
| **Cambio Principal** | [Se eliminó X campo del Body, se agregó Y Header, etc.]                    |
