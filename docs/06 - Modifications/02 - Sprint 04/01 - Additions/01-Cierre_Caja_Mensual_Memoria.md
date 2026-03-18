# 1. Servicio de Cierre de Caja Mensual

**Sprint:** 04
**Tipo:** Addition
**Fecha:** 20/02/2026
**Módulo:** Contabilidad

## Descripción

Se implementó un sistema para "cerrar" períodos fiscales mensuales. Cuando un período está cerrado, no se pueden agregar o modificar egresos a ese mes. Esta implementación utiliza almacenamiento en memoria como solución temporal.

**IMPORTANTE:** Todo está en memoria. Los períodos cerrados se pierden al reiniciar Docker.

## Archivos creados (7)

### DTOs

1. `src/Consulcon.Application/DTOs/Contabilidad/FiscalPeriods/ClosePeriodRequest.cs`
   - Request para cerrar un período (condominioId, year, month)
2. `src/Consulcon.Application/DTOs/Contabilidad/FiscalPeriods/FiscalPeriodDto.cs`
   - DTO que representa un período fiscal

### Interfaz

3. `src/Consulcon.Application/Interfaces/Contabilidad/IFiscalPeriodService.cs`
   - Interface del servicio con métodos:
     - `ClosePeriod()` - Cerrar período
     - `IsPeriodClosed()` - Verificar si está cerrado
     - `GetClosedPeriods()` - Listar períodos cerrados
     - `GetAuditLog()` - Ver log de auditoría

### Servicios

4. `src/Consulcon.Infrastructure/Services/Contabilidad/InMemoryFiscalPeriodStore.cs`
   - Singleton que guarda los períodos cerrados en un ConcurrentDictionary
   - También guarda un log de auditoría en memoria
5. `src/Consulcon.Infrastructure/Services/Contabilidad/FiscalPeriodService.cs`
   - Implementación del servicio
   - Validaciones: no cerrar períodos futuros, no cerrar dos veces

### Middleware

6. `src/Consulcon.API/Middleware/PeriodLockMiddleware.cs`
   - Intercepta POST/PUT/DELETE en `/api/expenses` y `/api/tesoreria/egresos`
   - Lee el body, extrae condominioId y fecha
   - Si el período está cerrado, retorna 400 con error "PERIOD_CLOSED"

### Controller

7. `src/Consulcon.API/Controllers/Contabilidad/FiscalPeriodsController.cs`
   - `POST /api/fiscal-periods/close` - Cerrar período
   - `GET /api/fiscal-periods/{condominioId}` - Listar períodos cerrados
   - `GET /api/fiscal-periods/{condominioId}/{year}/{month}` - Estado de un período
   - `GET /api/fiscal-periods/{condominioId}/audit-log` - Ver auditoría

## Archivos modificados

### Registros

Se agregó el registro en la inyección de dependencias `services.AddSingleton<InMemoryFiscalPeriodStore>()` y `services.AddScoped<IFiscalPeriodService, FiscalPeriodService>()`.
El middleware se conectó en `Program.cs` mediante `app.UseMiddleware<PeriodLockMiddleware>()`.

## Endpoints REST (Postman)

Rutas actualizadas bajo **Carpeta: Contabilidad > FiscalPeriods**:
| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/fiscal-periods/close` | Cierra un período fiscal |
| `GET` | `/api/fiscal-periods/{condominioId}` | Lista períodos cerrados |
| `GET` | `/api/fiscal-periods/{condominioId}/{year}/{month}` | Estado de período específico |
| `GET` | `/api/fiscal-periods/{condominioId}/audit-log` | Registro de auditoría |

## Consideraciones para Migración a BD

Cuando se decida persistir en base de datos:

1. Crear entidades de dominio `FiscalPeriod` y `FiscalPeriodAuditLog`.
2. Agregar DbSets correspondientes.
3. Modificar `FiscalPeriodService` para usar EF Core.
4. Eliminar `InMemoryFiscalPeriodStore`.
