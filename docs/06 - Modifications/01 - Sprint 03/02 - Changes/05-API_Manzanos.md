# 05. API de Manzanos - Implementación

**Sprint:** 03
**Tipo:** Change
**Fecha:** 05/02/2026
**Módulo:** Inmuebles

## Descripción

Se ha implementado la gestión completa de **Manzanos** (bloques) en el backend. Esta entidad permite agrupar propiedades dentro de un condominio y es fundamental para la organización física de las unidades.

## Cambios en Infraestructura

### 1. Dependency Injection

Ubicación: `src/Consulcon.Application/DependencyInjection.cs`

- **Modificación**: Registro del servicio `IManzanoService` en el contenedor de dependencias.
  ```csharp
  // Inmuebles
  services.AddScoped<IManzanoService, ManzanoService>();
  ```

## Cambios en Lógica de Negocio

### 1. DTOs

Ubicación: `src/Consulcon.Application/DTOs/Inmuebles/ManzanoDto.cs`

- **Nueva Funcionalidad**: Se creó `ManzanoDto` para la transferencia de datos.
  - Campos: `IdManzano`, `IdCondominio`, `Codigo`, `Nombre`.

### 2. Interfaces

Ubicación: `src/Consulcon.Application/Interfaces/Inmuebles/IManzanoService.cs`

- **Nueva Funcionalidad**: Definición del contrato para la gestión de manzanos.
  - `GetAllAsync()`
  - `GetByIdAsync(int id)`
  - `GetByCondominioAsync(int condominioId)`
  - `CreateAsync(ManzanoDto dto)`
  - `UpdateAsync(int id, ManzanoDto dto)`
  - `DeleteAsync(int id)`

### 3. Servicios

Ubicación: `src/Consulcon.Application/Services/Inmuebles/ManzanoService.cs`

- **Nueva Funcionalidad**: Implementación de la lógica de negocio utilizando el patrón `IRepository<Manzano>`.
- **Reglas**:
  - Permite filtrar manzanos por condominio específico.
  - Utiliza el repositorio genérico para operaciones CRUD estándar.

### 4. Controladores

Ubicación: `src/Consulcon.API/Controllers/Inmuebles/ManzanoController.cs`

- **Nueva Funcionalidad**: Exposición de los endpoints RESTful:
  - `GET /api/Manzano`
  - `GET /api/Manzano/{id}`
  - `GET /api/Manzano/condominio/{condominioId}`
  - `POST /api/Manzano`
  - `PUT /api/Manzano/{id}`
  - `DELETE /api/Manzano/{id}`

## Impacto

- **Postman**: Se actualizó la colección `docs/99 - Otros/02-postman/postman_collection.json` agregando la carpeta "Manzano" con sus respectivas peticiones.
- **Frontend**: Habilita la gestión de manzanos en la interfaz de usuario, permitiendo crear estructuras para las propiedades.
