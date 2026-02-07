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

- `[Name]Dto`: [Purpose, e.g., For reading].
- `Create[Name]Dto`: [Purpose, e.g., For creation].
- `Update[Name]Dto`: [Purpose, e.g., For update].

## Nuevo Controller

Ubicación: `src/Consulcon.API/Controllers/[Folder]/[Name]Controller.cs`

- Ruta Base: `api/[ControllerName]`

### Endpoints

#### [Endpoint Group]

- `GET [Route]`: [Description].
- `POST [Route]`: [Description].
- `PUT [Route]`: [Description].
- `DELETE [Route]`: [Description].

## Servicios

Ubicación: `src/Consulcon.Infrastructure/Services/[Name]Service.cs`

- Implementa `I[Name]Service`.
- **Funcionalidad**: [Describe the business logic handled here].
- **Interacción DB**: [Describe how it interacts with the database, repositories used, etc.].
Egreso.cs