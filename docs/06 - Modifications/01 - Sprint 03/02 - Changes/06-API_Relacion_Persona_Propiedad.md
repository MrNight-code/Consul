# 06. API de Asignación de Participantes (Unificada) - Implementación

**Sprint:** 03
**Tipo:** Change
**Fecha:** 05/02/2026
**Módulo:** Inmuebles / Ownership

## Descripción

Se ha refactorizado la lógica de asignación para unificar la gestión de propietarios, inquilinos y otros residentes en un único endpoint. Dependiendo del rol especificado, el sistema determina si se trata de una transferencia de titularidad (cerrando el ciclo anterior) o una simple asignación de nuevo participante.

## Cambios en Lógica de Negocio

### 1. DTOs

Ubicación: `src/Consulcon.Application/DTOs/Inmuebles/AssignParticipantDto.cs`

- **Nueva Estructura Unificada**:
  - Campos:
    - `PropiedadId`: ID de la propiedad.
    - `PersonaId`: ID de la persona.
    - `Rol`: "PROPIETARIO", "INQUILINO", "GARANTE", etc.
    - `ContratoId`: (Opcional) ID del contrato específico.
    - `FechaInicio`: Fecha de alta.
    - `FechaFin`: (Opcional) Fecha de baja.
    - `Observaciones`: Notas.

### 2. Servicios

Ubicación: `src/Consulcon.Infrastructure/Services/Inmuebles/OwnershipService.cs`

- **Método**: `AssignParticipantAsync`
- **Lógica**:
  - **Si Rol es PROPIETARIO**: Ejecuta la lógica "Transferencia" (Cierra el propietario anterior, crea uno nuevo).
  - **Si Rol es OTRO (ej. INQUILINO)**: Ejecuta la lógica "Adición" (Agrega el participante al contrato vigente sin afectar al propietario).

### 3. Controladores

Ubicación: `src/Consulcon.API/Controllers/OwnershipController.cs`

- **Endpoint Unificado**: `POST /api/ownership/assign-participant`
- Reemplaza a los antiguos endpoints separados.

## Impacto

- **Simplificación**: Frontend solo necesita invocar un endpoint cambiando el parámetro `rol`.
- **Flexibilidad**: Mismo flujo para cualquier tipo de relación persona-propiedad.
