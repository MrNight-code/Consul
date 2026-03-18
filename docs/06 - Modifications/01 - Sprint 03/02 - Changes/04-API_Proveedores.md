# 04. API de Proveedores - Implementación

**Sprint:** 03
**Tipo:** Change
**Fecha:** 05/02/2026
**Módulo:** Contabilidad

## Descripción

Se ha implementado la gestión completa de **Proveedores** en el backend. Anteriormente, la entidad `Proveedor` existía en el dominio (`Consulcon.Domain`) pero no contaba con una capa de servicios ni controladores expuestos en la API.

> [!NOTE]
> **Aclaración Importante sobre Empresas:**
> En el contexto del sistema, las "Empresas" que prestan servicios o suministros al condominio se gestionan a través de la entidad **Proveedor**. Por lo tanto, cualquier requerimiento funcional relacionado con "Empresas" debe ser atendido utilizando los endpoints de Proveedores.

## Cambios en Infraestructura

### 1. Dependency Injection

Ubicación: `src/Consulcon.Application/DependencyInjection.cs`

- **Modificación**: Registro del servicio `IProveedorService` en el contenedor de dependencias.
  ```csharp
  // Contabilidad
  services.AddScoped<IProveedorService, ProveedorService>();
  ```

## Cambios en Lógica de Negocio

### 1. DTOs

Ubicación: `src/Consulcon.Application/DTOs/Contabilidad/ProveedorDto.cs`

- **Nueva Funcionalidad**: Se creó `ProveedorDto` para transferir los datos de proveedores de manera segura.
  - Campos: `IdProveedor`, `RazonSocial`, `Nit`, `Contacto`, `Direccion`, `Activo`.

### 2. Interfaces

Ubicación: `src/Consulcon.Application/Interfaces/Contabilidad/IProveedorService.cs`

- **Nueva Funcionalidad**: Definición del contrato para la gestión de proveedores.
  - `GetAllAsync()`
  - `GetByIdAsync(int id)`
  - `CreateAsync(ProveedorDto dto)`
  - `UpdateAsync(int id, ProveedorDto dto)`
  - `DeleteAsync(int id)`

### 3. Servicios

Ubicación: `src/Consulcon.Application/Services/Contabilidad/ProveedorService.cs`

- **Nueva Funcionalidad**: Implementación de la lógica de negocio utilizando el patrón `IRepository<Proveedor>`.
- **Flujo de Datos**: Interactúa directamente con la tabla de Proveedores mediante el repositorio genérico. Incluye validaciones básicas de existencia.

### 4. Controladores

Ubicación: `src/Consulcon.API/Controllers/Contabilidad/ProveedorController.cs`

- **Nueva Funcionalidad**: Exposición de los endpoints RESTful:
  - `GET /api/Proveedor`
  - `GET /api/Proveedor/{id}`
  - `POST /api/Proveedor`
  - `PUT /api/Proveedor/{id}`
  - `DELETE /api/Proveedor/{id}`

## Impacto

- **Postman**: Se actualizó la colección `docs/99 - Otros/02-postman/postman_collection.json` agregando la carpeta "Proveedores" con ejemplos de todas las peticiones.
- **Frontend**: El frontend ahora puede consumir estos endpoints para listar y gestionar las empresas/proveedores del sistema.
