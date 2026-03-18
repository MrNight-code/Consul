# 07. Fix: Creación de Condominio con Persistencia Completa

**Sprint:** 03
**Tipo:** Change
**Fecha:** 06/02/2026
**Módulo:** Inmuebles

## Descripción

Se solucionó la persistencia incompleta en la creación de Condominios. Anteriormente, solo se guardaba el registro maestro (`CondominioMaster`), perdiendo detalles como dirección, logo y configuración que deben ir en la base de datos del Tenant. Además, se corrigió el DTO para usar `IdCondominio` y se eliminó la dependencia directa de Infraestructura en la Capa de Aplicación.

## Cambios en Infraestructura

### 1. `TenantDatabaseService`

Ubicación: `src/Consulcon.Infrastructure/Persistence/Services/TenantDatabaseService.cs`

- **Modificación**: Se implementó el método `InitializeCondominioAsync`. Además de guardar el Condominio, este método **crea automáticamente un registro de Persona (Administrador)** en la nueva base de datos para satisfacer la integridad referencial (Foreign Key `IdAdminPersona`).

  ```csharp
  public async Task InitializeCondominioAsync(string databaseName, Application.DTOs.Inmuebles.CondominioDto initialData)
  {
      // 1. Crear Persona Admin Inicial (para evitar error FK)
      // 2. Crear Condominio vinculando la Persona
      // 3. Guardar cambios en BD Tenant
  }
  ```

## Cambios en Lógica de Negocio

- **Servicio Afectado**: `CondominioService` (`src/Consulcon.Application/Services/Inmuebles/CondominioService.cs`)
- **Nueva Funcionalidad**:
  - El método `CreateAsync` ahora orquesta la creación del Master, la provisión de la BD del Tenant y la inicialización de los datos del Condominio en dicha BD.
  - Se eliminó el código que intentaba instanciar `ConsulconDbContext` directamente.
- **Flujo de Datos**:
  1. Se crea `CondominioMaster`.
  2. Se genera el `TenantId` y nombre de BD.
  3. Se solicita a Infraestructura crear la BD y correr migraciones.
  4. Se solicita a Infraestructura guardar los datos detallados del Condominio (`InitializeCondominioAsync`).

### Refactorización de DTOs

- **Archivo**: `CondominioDto.cs`
- **Cambio**: Renombrado `Id` -> `IdCondominio`.

## Impacto

- **API**: El endpoint `POST /api/Condominio` ahora devuelve el objeto completo con datos persistidos (ya no devuelve nulls).
- **Postman**: Se actualizó la colección para no enviar `IdCondominio` ni `Codigo` en el cuerpo del request, ya que son auto-generados. Se fusionaron las carpetas de Proveedores y se añadió el request para "Add User to Condominio".
- **Tests**: Se actualizaron los tests de integración para reflejar el cambio de nombre de la propiedad ID.

## Fix Adicional (Schema Change)

Para solucionar que el endpoint `GET /api/Condominio` devolvía valores nulos en propiedades informativas (Logo, Dirección, ConfigDiaCobro), se extendió la entidad `CondominioMaster`.

- **Tabla Modificada**: `CondominiosMaster`
- **Nuevas Columnas**:
  - `Direccion` (varchar 200)
  - `Logo` (varchar 500)
  - `ConfigDiaCobro` (varchar 50)
  - `SuperficieTotalM2` (decimal 18,2)
  - `ConnectionString` (ya existía pero se hizo explícito el mapeo)

Esto permite que el Dashboard en el panel de control muestre la información correcta sin consultar cada base de datos de inquilino individualmente.
