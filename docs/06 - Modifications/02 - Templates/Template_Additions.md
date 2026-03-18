# [Number]. [Feature Name]

**Sprint:** [Sprint Number]
**Tipo:** Addition
**Fecha:** [DD/MM/YYYY]

## Visión General

[Short description of what this task implements and its main purpose. Mention if there is Multi-tenancy support or other key features.]

## Nuevas Entidades

### 1. `[EntityName]` ([Spanish Name/Description])

Ubicación: `src/Consulcon.Domain/Entities/[Folder]/[EntityName].cs`

- **Propósito**: [Explain what this entity represents in the business domain].
- **Relación DB**: [Explain if it resides in Master or Tenant DB, and its key relationships to other entities].
- **Propiedades Clave**:
  - `Id`: Identificador único.
  - `[Property]`: [Description].

## Nuevos DTOs

Ubicación: `src/Consulcon.Application/DTOs/[Folder]/`

| DTO               | Propósito                     | Campos Clave                 |
| ----------------- | ----------------------------- | ---------------------------- |
| `[Name]Dto`       | [Purpose, e.g., For reading]  | `Id`, `Nombre`               |
| `Create[Name]Dto` | [Purpose, e.g., For creation] | `Nombre`, `CamposRequeridos` |
| `Update[Name]Dto` | [Purpose, e.g., For update]   | `Nombre`                     |

## Nuevo Controller

Ubicación: `src/Consulcon.API/Controllers/[Folder]/[Name]Controller.cs`

- Ruta Base: `api/[ControllerName]`

### Endpoints

#### 1. [Nombre del Endpoint]

| Propiedad       | Valor                                      |
| --------------- | ------------------------------------------ |
| **Método**      | `POST` / `GET` / `PUT` / `DELETE`          |
| **Ruta**        | `api/[ControllerName]/[Route]`             |
| **Descripción** | [Descripción de lo que hace el endpoint]   |
| **Parámetros**  | [Lista de parámetros, ej: `id` (int)]      |
| **Body (JSON)** | [Descripción del body o esquema]           |
| **Respuesta**   | [Qué retorna el endpoint en caso de éxito] |

## Servicios

Ubicación: `src/Consulcon.Infrastructure/Services/[Name]Service.cs`

- Implementa `I[Name]Service`.
- **Funcionalidad**: [Describe the business logic handled here].
- **Interacción DB**: [Describe how it interacts with the database, repositories used, etc.].

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02 - Postman/postman_collection.json`  
**Carpeta:** `[Entities Folder] > [EntityName]`

### Request 1: [Request Name]

| Campo           | Valor                                                                      |
| --------------- | -------------------------------------------------------------------------- |
| **Nombre**      | [Request Name]                                                             |
| **Método**      | `[HTTP Method]`                                                            |
| **URL**         | `{{baseUrl}}/api/[Route]`                                                  |
| **Headers**     | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}` |
| **Body (JSON)** | `{ "[field]": "[value]" }`                                                 |
