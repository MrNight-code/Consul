# 01. Refactor Endpoints de Gestión de Condominios

**Sprint:** 03
**Tipo:** Change
**Fecha:** 26/01/2026
**Módulo:** Inmuebles (Condominios)

## Descripción

Se ha refactorizado la gestión de Condominios (`GET` y `POST` en `/api/Condominio`) para desacoplarse del contexto de Base de Datos del Inquilino (Tenant DB) y operar directamente sobre la Base de Datos Maestra (`MasterDbContext`).

## ¿Qué entidades usa la DB y Cómo funciona?

Este cambio introduce un flujo híbrido entre la Base Maestra y las Bases de Inquilinos.

### 1. `CondominioMaster` (Base Maestra)

- **Ubicación**: `Consulcon.Domain.Entities.Master.CondominioMaster`
- **Tabla**: `CondominiosMaster` (en BD principal/System)
- **Función**: Actúa como el directorio central. Guarda el `ID`, `Nombre` y la `ConnectionString` (o TenantId) para saber a qué base de datos conectarse.
- **Uso**: Cuando un usuario lista sus condominios, la info sale de AQUÍ, porque es rápido y centralizado.

### 2. `UsuarioCondominio` (Base Maestra)

- **Ubicación**: `Consulcon.Domain.Entities.Master.UsuarioCondominio`
- **Tabla**: `UsuarioCondominio` (en BD principal/System)
- **Función**: Tabla de relación muchos-a-muchos. Dice "¿Qué usuarios tienen acceso a qué condominios?".
- **Uso**: El endpoint `GET` usa esto para filtrar "Mis Condominios".

### 3. `Condominio` (Base Inquilino)

- **Ubicación**: `Consulcon.Domain.Entities.Inmuebles.Condominio`
- **Tabla**: `Condominios` (en BD `db_condominio_X`)
- **Función**: Guarda los detalles finos del condominio (Logo, Configuración de Día de Cobro, Dirección completa) dentro de su propia base de datos aislada.

## Cambios Realizados

### Backend

1.  **Entidades de Dominio**:
    - Se utilizan las entidades `CondominioMaster` y `UsuarioCondominio` (ubicadas en `Consulcon.Domain.Entities.Master`) para las operaciones principales.

2.  **Servicio `CondominioService`**:
    - **Implantación Actualizada**: El servicio ahora inyecta repositorios para las entidades maestras en lugar del repositorio genérico del tenant.
    - **GET /api/Condominio**: Recupera la lista de condominios asignados al usuario directamente de `UsuarioCondominio` en la BD Maestra.
    - **POST /api/Condominio**:
      1. Crea el registro en `CondominioMaster`.
      2. Asigna al usuario creador como 'Administrador' en `UsuarioCondominio`.
      3. Inicializa (CREA) la base de datos del inquilino dinámicamente (`db_condominio_{id}`).

3.  **Controlador `CondominioController`**:
    - Se actualizó para extraer el `UserId` de los Claims del token JWT (`ClaimTypes.NameIdentifier` o `sub`) y pasarlo al servicio.

## Impacto

- **Listado Global**: Los usuarios ahora pueden ver su lista de condominios sin necesidad de estar conectados a un contexto de condominio específico previamente, facilitando la pantalla de selección de condominio.
- **Creación Centralizada**: La creación de condominios es ahora una operación de nivel "Sistema" que aprovisiona los recursos necesarios.

## Notas Técnicas

- El DTO `CondominioDto` devuelto en el listado puede tener campos nulos (como `Direccion` detallada o `Logo`) si estos residen exclusivamente en la BD del Tenant y no en la Maestra. Solo los datos esenciales (`Id`, `Nombre`, `TenantId`) están garantizados en el listado maestro.

## Preguntas Frecuentes (FAQ Técnico)

### ¿Se modificó la estructura de la Base de Datos?

**No.** Este cambio fue puramente de lógica en el Backend.

- Se reutilizaron tablas existentes en la Base Maestra (`CondominiosMaster`, `UsuarioCondominio`).
- No se agregaron columnas ni tablas nuevas para este refactor.
