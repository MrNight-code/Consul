# 02. Global Roles & Endpoints - Adiciones

**Sprint:** 04
**Tipo:** Addition
**Fecha:** 25/02/2026
**Módulo:** Seguridad

## Descripción

Se implementó la arquitectura global en la base de datos `Master` para la gestión centralizada de `Roles` y `Permisos`, permitiendo desvincular estas políticas del entorno `Tenant` (condominios específicos).

## Nuevas Entidades Dominio (Master)

Se agregaron las definiciones de las siguientes propiedades dentro del archivo `MasterEntities.cs` localizado en el Dominio global:

### 1. `RolMaster`

- Entidad global con `IdRol`, `Nombre` y `Descripcion`.
- Sustituye a la anterior entidad local de Rol.

### 2. `PermisoMaster`

- Entidad global con `IdPermiso`, `Nombre` y `Descripcion`.
- Gestiona los permisos disponibles para el sistema global de `Consulcon`.

**Ubicación:** `src/Consulcon.Domain/Entities/Master/MasterEntities.cs`

## Nueva Arquitectura de Servicio (`RolService`)

Se creó una estructura de servicio completamente nueva para consultar los roles y servir al nuevo Endpoint en capa global:

### 1. `IRolService` y `RolDto`

- Creados en `Consulcon.Application` para establecer el contrato del Endpoint y la transferencia del Id y Nombre del Rol.

### 2. `RolService` (Infrastructure)

- Se implementó dentro de la infraestructura (`src/Consulcon.Infrastructure/Services/Seguridad/RolService.cs`) usando `Dapper`.
- **Importante:** Esta clase inyecta su cadena de conexión apuntando rígidamente y siempre a `db_consulcon_master` para asegurar que el Endpoint liste correctamente los roles sin depender de la resolución del Tenant a través del middleware.

## Nuevos Endpoints (Controladores)

### `RolController`

Se expuso el listado de roles a través del controlador global `RolController.cs`:

- **Endpoint:** `GET /api/Rol`
- **Autorización:** `[Authorize(Policy = "SuperAdminOnly")]` (Opcional según política actual, pero estructurado para global).
- **Funcionalidad:** Devuelve los roles base para armar comboboxes en el frontend (ej. _Super Admin_, _Administrador_, _Operador_).

**Ubicación:** `src/Consulcon.API/Controllers/Seguridad/RolController.cs`
