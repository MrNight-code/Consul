# 04. Motor de Asignación y Auditoría de Propiedad

**Sprint:** Sprint 03
**Tipo:** Addition
**Fecha:** 27/01/2026

## Visión General

Implementación de un motor transaccional para la asignación de propietarios a unidades inmobiliarias (`Propiedad`). Permite registrar cambios de dueño manteniendo un historial inalterable para cumplir con auditorías y resolver disputas de deudas antiguas.

**Características clave:**

- Reutiliza las entidades existentes `Contrato` y `ContratoParticipante`.
- Lógica transaccional: cierra el registro vigente y crea el nuevo atómicamente.
- Validación de solapamiento de fechas.
- Sin hardcoding: usa constantes definidas en `OwnershipConstants`.

## Nuevas Entidades

> **Nota:** No se crearon nuevas entidades de base de datos. Se reutilizan las existentes.

### Entidades Reutilizadas

| Entidad                | Ubicación                                    | Uso                                                               |
| ---------------------- | -------------------------------------------- | ----------------------------------------------------------------- |
| `Propiedad`            | `Entities/Inmuebles/Propiedad.cs`            | La unidad inmobiliaria                                            |
| `Contrato`             | `Entities/Contratos/Contrato.cs`             | Vínculo entre propiedad y participantes                           |
| `ContratoParticipante` | `Entities/Contratos/ContratoParticipante.cs` | Registra al dueño (RolContrato="Titular") con FechaAlta/FechaBaja |

### Nuevo Archivo: Constantes

Ubicación: `src/Consulcon.Domain/Constants/OwnershipConstants.cs`

```csharp
public static class OwnershipConstants
{
    public const string RolTitular = "Titular";
    public const string RolInquilino = "Inquilino";
    public const string RolGarante = "Garante";
    public const string EstadoVigente = "VIGENTE";
    public const string EstadoFinalizado = "Finalizado";
    public const string EstadoRescindido = "Rescindido";
}
```

## Nuevos DTOs

Ubicación: `src/Consulcon.Application/DTOs/Inmuebles/`

- `AssignOwnerDto`: Para la solicitud de asignación de propietario.
  - `PropiedadId`: ID de la unidad.
  - `NuevoDuenoId`: ID de la persona (nuevo dueño).
  - `FechaInicio`: Fecha de inicio de la titularidad.
  - `Observaciones`: Observaciones opcionales.

- `OwnershipHistoryDto`: Para el historial de propietarios.
  - `ContratoId`, `PersonaId`, `NombrePersona`
  - `FechaInicio`, `FechaFin`, `EsVigente`

## Nuevo Controller

Ubicación: `src/Consulcon.API/Controllers/OwnershipController.cs`

- Ruta Base: `api/ownership`

### Endpoints

| Método | Ruta                                   | Descripción                                   |
| ------ | -------------------------------------- | --------------------------------------------- |
| `POST` | `/api/ownership/assign-owner`          | Asigna nuevo propietario (transaccional)      |
| `GET`  | `/api/ownership/history/{propiedadId}` | Obtiene historial cronológico de propietarios |

#### POST /api/ownership/assign-owner

**Request Body:**

```json
{
  "propiedadId": 1,
  "nuevoDuenoId": 5,
  "fechaInicio": "2026-02-01",
  "observaciones": "Transferencia por compra-venta"
}
```

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": {
    "contratoId": 1,
    "personaId": 5,
    "nombrePersona": "Juan Pérez",
    "fechaInicio": "2026-02-01",
    "fechaFin": null,
    "esVigente": true
  },
  "message": "Propietario asignado exitosamente."
}
```

#### GET /api/ownership/history/{propiedadId}

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "data": [
    {
      "contratoId": 1,
      "personaId": 5,
      "nombrePersona": "Juan Pérez",
      "fechaInicio": "2026-02-01",
      "fechaFin": null,
      "esVigente": true
    },
    {
      "contratoId": 1,
      "personaId": 3,
      "nombrePersona": "María García",
      "fechaInicio": "2024-01-15",
      "fechaFin": "2026-01-31",
      "esVigente": false
    }
  ]
}
```

## Servicios

Ubicación: `src/Consulcon.Infrastructure/Services/Inmuebles/OwnershipService.cs`

- Implementa `IOwnershipService`.
- **Funcionalidad**:
  1. `AssignOwnerAsync`: Asigna nuevo propietario transaccionalmente.
     - Valida existencia de `Propiedad` y `Persona`.
     - Busca contrato vigente (o lo crea si no existe).
     - Cierra el registro actual de "Titular" (`FechaBaja = FechaInicio - 1 día`).
     - Inserta nuevo registro de "Titular".
     - Transacción garantiza atomicidad.
  2. `GetOwnershipHistoryAsync`: Devuelve historial completo ordenado cronológicamente.

- **Interacción DB**: Usa `ConsulconDbContext` directamente con transacciones explícitas.

## Criterios de Aceptación

| Criterio                                                           | Estado |
| ------------------------------------------------------------------ | ------ |
| La base de datos refleja el cambio sin borrar registros anteriores | ✅     |
| Se validan solapamientos de fechas                                 | ✅     |
| Endpoint de consulta devuelve historial cronológico                | ✅     |
