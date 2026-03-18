# 03. Global Roles & UserDto Refactor - Cambios

**Sprint:** 04
**Tipo:** Change / Refactor
**Fecha:** 25/02/2026
**Módulo:** Seguridad

## Descripción

Se migró exitosamente toda la arquitectura de validación de `Roles` a un entorno global (Master), deshaciéndola del contexto Tenant en `ConsulconDbContext`. También se ha refactorizado la generación de tokens para utilizar el atributo `Email` en sustitución del antiguo `FullName`.

## Cambios en Infraestructura

### 1. `ConsulconDbContext`

Ubicación: `src/Consulcon.Infrastructure/Persistence/ConsulconDbContext.cs`

- **Modificación**: Se eliminó la configuración de propiedades ligadas al inquilino (`Roles`, `Permisos`, `RolPermiso`). Se ha configurado el DbContext para ser inyectable en el `RolService` global de manera que se puedan consultar `RolMaster` desde la BD maestra.
- **Relacionado**: Se integró un sembrado inicial del Rol `"Super Admin"` (Id 1) al usuario principal `admin` en el `DatabaseMigrationInitializer.cs` para su asignación automatizada durante el inicio.

### 2. Dependency Injection

Ubicación: `src/Consulcon.Application/DependencyInjection.cs`

- **Modificación**: Se ha inyectado formalmente el `RolService` en el bloque asociado al módulo de Seguridad.
  ```csharp
  services.AddScoped<IRolService, RolService>();
  ```

## Cambios en DTOs

### `UserDto` (Modificado)

| Campo           | Cambio         | Descripción                                                                          |
| --------------- | -------------- | ------------------------------------------------------------------------------------ |
| `Token`         | **Modificado** | Se agregó a condición `JsonIgnoreCondition.WhenWritingNull` para la lista de lectura |
| `Tenants`       | **Eliminado**  | Removido debido a ineficiencia visual en los logs y objetos pesados.                 |
| `CondominioIds` | **Agregado**   | Lista de integros (`List<int>`) mapeados limpiamente en reemplazo a Tenants.         |
| `FullName`      | **Eliminado**  | Removido obsoleto.                                                                   |
| `Email`         | **Agregado**   | Correo integrado jalando el campo nativo de base de datos desde la Persona o Master. |

**Ubicación:** `src/Consulcon.Application/DTOs/Seguridad/UserDto.cs`

## Endpoints Modificados

### 1. Obtener Roles Base

| Propiedad             | Valor                                                         |
| --------------------- | ------------------------------------------------------------- |
| **Método**            | `GET`                                                         |
| **Ruta**              | `/api/Rol`                                                    |
| **Descripción**       | El Endpoint ya responde exitosamente sin error HTTP 500       |
| **Body / Parámetros** | Se hizo exenta de la validación del Header `X-Condominio-Id`. |

### 2. Generación e Indexación del JWT Login

| Propiedad             | Valor                                                                       |
| --------------------- | --------------------------------------------------------------------------- |
| **Método**            | `POST`                                                                      |
| **Ruta**              | `/api/Auth/Login`                                                           |
| **Descripción**       | El Token retornado ahora incorpora los Claims en formato global de tenants. |
| **Body / Parámetros** | El Payload nativo incorpora la decodificación del Token como `email`.       |

## Cambios en Lógica de Negocio

- **Servicio Afectado**: `RolService.cs`, `AuthService.cs` y `JwtTokenGenerator.cs`
- **Nuevos Cambios**: El `RolService` original en Application que usaba Entity Framework y dependía del Tenant fue eliminado. En su lugar se programó con **Dapper Query** un conector hardcodeado y explícito en `Infrastructure` hacia la DB `db_consulcon_master`, solucionando el problema de array vacío `[]` provocado por middlewares.
- **Sincronizador UserSyncer**: Se ajustó el script legacy en `scripts/UserSyncer/Program.cs` para recolectar correctamente el `id_rol_principal` antiguo, parcheando el default a `IdRol = 3` (Operador) y asignándolo correctamente a `UsuarioCondominio`.
- **Flujo de Datos JWT**: El JWT inyecta un Claim explícito guardando `Email` en vez del antiguo Nombre de la asociación, y se expone `EsSuperAdmin`.

## Impacto

- Este cambio fuerza que los reportes o validaciones Front-End dejen de hacer tracking o parsing con `Tenants` y los Claims de `FullName` optando de manera limpia a `email`.
- Toda la base de pruebas `IntegrationTests` ha sido modificada reemplazando la simulación previa de los roles mediante las variables de testing integrales para compilar.

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02 - Postman/postman_collection.json`  
**Carpeta:** `Seguridad / Configuración`

### Request 1: Autenticación Admin (Modificado)

| Campo                | Valor                                                                        |
| -------------------- | ---------------------------------------------------------------------------- |
| **Nombre**           | Admin Login Global                                                           |
| **Método**           | `POST`                                                                       |
| **URL**              | `{{baseUrl}}/api/Auth/Login`                                                 |
| **Headers**          | Nada extra para Auth Global.                                                 |
| **Cambio Principal** | Dejará de retonar el listado gigantesco de Tenant para dar un array numérico |

### Request 2: Obtener Roles (Modificado)

| Campo                | Valor                                                                     |
| -------------------- | ------------------------------------------------------------------------- |
| **Nombre**           | Get Roles                                                                 |
| **Método**           | `GET`                                                                     |
| **URL**              | `{{baseUrl}}/api/Rol`                                                     |
| **Headers**          | Se remueve explicitamente `X-Condominio-Id` de los headers globales.      |
| **Cambio Principal** | Gracias a Dapper, el Endpoint ya no requiere Condominio Id en el Postman. |
