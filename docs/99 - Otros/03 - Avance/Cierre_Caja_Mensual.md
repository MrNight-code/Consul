# Avance: Servicio de Cierre de Caja Mensual
**Fecha:** 2026-02-07
**Desarrollado con:** Gemini AI Assistant

---

## Qué se hizo

Se implementó un sistema para "cerrar" períodos fiscales mensuales. Cuando un período está cerrado, no se pueden agregar egresos a ese mes.

**IMPORTANTE:** Todo está en memoria, no se tocó la base de datos. Los períodos cerrados se pierden al reiniciar Docker.

---

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

---

## Archivos modificados (3)

### DependencyInjection.cs
Se agregaron 3 líneas para registrar los servicios:
```csharp
services.AddSingleton<InMemoryFiscalPeriodStore>();
services.AddScoped<IFiscalPeriodService, FiscalPeriodService>();
```

### Program.cs
Se agregó el middleware después de UseAuthorization:
```csharp
app.UseMiddleware<PeriodLockMiddleware>();
```

### postman_collection.json
Se agregó carpeta "FiscalPeriods" con 4 endpoints para probar

---

## Cómo probar

1. Hacer login en Postman para obtener token
2. Ir a FiscalPeriods > Close Period
3. Enviar:
```json
{
  "condominioId": 1,
  "year": 2026,
  "month": 1
}
```
4. Intentar crear un egreso con fecha en enero 2026 → debería fallar

---

## Limitaciones

- Los datos están en memoria, se pierden al reiniciar
- No hay endpoint para reabrir un período cerrado
- El middleware solo protege rutas específicas (/api/expenses, /api/tesoreria/egresos)
