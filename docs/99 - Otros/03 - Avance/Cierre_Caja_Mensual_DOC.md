# Documentación Técnica: Servicio de Cierre de Períodos Fiscales

**Versión:** 1.0  
**Fecha:** 2026-02-07  
**Estado:** Implementado (En Memoria)

---

## 1. Resumen Ejecutivo

Se implementó un servicio de cierre de caja mensual que permite a los administradores "cerrar" períodos fiscales para prevenir la modificación de egresos en meses pasados. Esta implementación utiliza almacenamiento en memoria como solución temporal según requerimientos del equipo.

### Decisiones de Diseño Clave
- **Sin persistencia en BD**: Por restricción del equipo, los datos se almacenan en memoria
- **Singleton para almacenamiento**: Garantiza consistencia durante el ciclo de vida de la aplicación
- **Middleware para validación**: Intercepta requests antes de llegar al controller
- **Thread-safe**: Uso de `ConcurrentDictionary` para operaciones concurrentes

---

## 2. Arquitectura

### 2.1 Diagrama de Capas

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                             │
│  ┌─────────────────────┐  ┌─────────────────────────────┐   │
│  │ PeriodLockMiddleware │  │ FiscalPeriodsController     │   │
│  │ (Interceptor)        │  │ (Endpoints)                 │   │
│  └──────────┬──────────┘  └─────────────┬───────────────┘   │
└─────────────┼───────────────────────────┼───────────────────┘
              │                           │
              ▼                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                         │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ IFiscalPeriodService                                 │    │
│  │   - ClosePeriod(request, userId)                     │    │
│  │   - IsPeriodClosed(condominioId, date)               │    │
│  │   - GetClosedPeriods(condominioId)                   │    │
│  │   - GetAuditLog(condominioId)                        │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
              │
              ▼
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ FiscalPeriodService : IFiscalPeriodService          │    │
│  └──────────────────────────┬──────────────────────────┘    │
│                             │                                │
│                             ▼                                │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ InMemoryFiscalPeriodStore (Singleton)               │    │
│  │   - ConcurrentDictionary<(int,int,int), Info>       │    │
│  │   - List<AuditEntry> (con lock)                     │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Flujo de Cierre de Período

```
Usuario                    Controller              Service                Store
   │                           │                      │                     │
   │  POST /close              │                      │                     │
   │ ─────────────────────────>│                      │                     │
   │                           │  ClosePeriod()       │                     │
   │                           │ ────────────────────>│                     │
   │                           │                      │  Validar período    │
   │                           │                      │ ───────────────────>│
   │                           │                      │                     │
   │                           │                      │  IsClosed()?        │
   │                           │                      │ ───────────────────>│
   │                           │                      │<───────── false ────│
   │                           │                      │                     │
   │                           │                      │  ClosePeriod()      │
   │                           │                      │ ───────────────────>│
   │                           │                      │                     │
   │                           │                      │  AddAuditEntry()    │
   │                           │                      │ ───────────────────>│
   │                           │<──── Result.Ok ─────│                     │
   │<────── 200 OK ───────────│                      │                     │
```

### 2.3 Flujo de Bloqueo de Escritura (Middleware)

```
Cliente                 Middleware               Service              Expense Controller
   │                        │                       │                        │
   │  POST /api/expenses    │                       │                        │
   │ ──────────────────────>│                       │                        │
   │                        │  Leer body JSON       │                        │
   │                        │  Extraer condominioId │                        │
   │                        │  Extraer fecha        │                        │
   │                        │                       │                        │
   │                        │  IsPeriodClosed()     │                        │
   │                        │ ─────────────────────>│                        │
   │                        │                       │                        │
   │                        │<────── true ─────────│                        │
   │                        │                       │                        │
   │<─── 400 Bad Request ───│                       │                        │
   │     PERIOD_CLOSED      │       (No llega al controller)                │
```

---

## 3. Componentes Implementados

### 3.1 DTOs (`Consulcon.Application/DTOs/Contabilidad/FiscalPeriods/`)

| Archivo | Descripción |
|---------|-------------|
| `ClosePeriodRequest.cs` | Request para cerrar un período |
| `FiscalPeriodDto.cs` | Representación de un período fiscal |

### 3.2 Interfaz (`Consulcon.Application/Interfaces/Contabilidad/`)

| Archivo | Descripción |
|---------|-------------|
| `IFiscalPeriodService.cs` | Contrato del servicio + `FiscalPeriodAuditDto` |

### 3.3 Servicios (`Consulcon.Infrastructure/Services/Contabilidad/`)

| Archivo | Ciclo de Vida | Descripción |
|---------|---------------|-------------|
| `InMemoryFiscalPeriodStore.cs` | **Singleton** | Almacenamiento thread-safe |
| `FiscalPeriodService.cs` | Scoped | Lógica de negocio |

### 3.4 API (`Consulcon.API/`)

| Archivo | Descripción |
|---------|-------------|
| `Middleware/PeriodLockMiddleware.cs` | Interceptor de escrituras |
| `Controllers/Contabilidad/FiscalPeriodsController.cs` | Endpoints REST |

---

## 4. Endpoints REST

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/api/fiscal-periods/close` | Cierra un período fiscal |
| `GET` | `/api/fiscal-periods/{condominioId}` | Lista períodos cerrados |
| `GET` | `/api/fiscal-periods/{condominioId}/{year}/{month}` | Estado de período específico |
| `GET` | `/api/fiscal-periods/{condominioId}/audit-log` | Registro de auditoría |

### 4.1 Ejemplo: Cerrar Período

**Request:**
```http
POST /api/fiscal-periods/close
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
Content-Type: application/json

{
  "condominioId": 1,
  "year": 2026,
  "month": 1
}
```

**Response (200 OK):**
```json
{
  "message": "Período 01/2026 cerrado exitosamente.",
  "period": {
    "condominioId": 1,
    "year": 2026,
    "month": 1,
    "isClosed": true,
    "closedAt": "2026-02-07T12:30:00Z",
    "closedByUserId": 1
  }
}
```

### 4.2 Ejemplo: Error por Período Cerrado

```http
POST /api/expenses
```

**Response (400 Bad Request):**
```json
{
  "isSuccess": false,
  "isFailure": true,
  "errorCode": "PERIOD_CLOSED",
  "message": "El período 01/2026 está cerrado. No se pueden agregar, modificar o eliminar egresos en períodos cerrados.",
  "period": { "year": 2026, "month": 1 }
}
```

---

## 5. Registro en Dependency Injection

```csharp
// DependencyInjection.cs (líneas agregadas)
services.AddSingleton<InMemoryFiscalPeriodStore>();
services.AddScoped<IFiscalPeriodService, FiscalPeriodService>();
```

```csharp
// Program.cs (línea agregada)
app.UseMiddleware<PeriodLockMiddleware>();
```

---

## 6. Consideraciones de Seguridad

### 6.1 Rutas Protegidas por el Middleware
- `/api/expenses` (POST, PUT, DELETE)
- `/api/tesoreria/egresos` (POST, PUT, DELETE)

### 6.2 Validaciones Implementadas
- No se puede cerrar un período futuro
- No se puede cerrar un período ya cerrado
- Solo usuarios autenticados pueden cerrar períodos

### 6.3 Auditoría
Cada cierre de período genera un registro con:
- Usuario que realizó el cierre
- Timestamp del cierre
- TraceId para correlación con logs

---

## 7. Limitaciones Conocidas

| Limitación | Impacto | Solución Futura |
|------------|---------|-----------------|
| Datos en memoria | Se pierden al reiniciar | Persistir en tabla `fiscal_periods` |
| Sin reapertura | Período cerrado es permanente | Agregar endpoint `/reopen` con validaciones |
| Rutas hardcoded | Nuevos endpoints de egresos no estarían protegidos | Configurar rutas en appsettings.json |

---

## 8. Consideraciones para Migración a BD

Cuando se decida persistir en base de datos:

### 8.1 Esquema Propuesto

```sql
CREATE TABLE fiscal_periods (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_condominio INT NOT NULL,
    year INT NOT NULL,
    month INT NOT NULL,
    is_closed BOOLEAN DEFAULT FALSE,
    closed_at DATETIME NULL,
    closed_by_user_id INT NULL,
    UNIQUE KEY uk_period (id_condominio, year, month),
    FOREIGN KEY (id_condominio) REFERENCES condominio(id_condominio),
    FOREIGN KEY (closed_by_user_id) REFERENCES usuario(id_usuario)
);

CREATE TABLE fiscal_period_audit_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fiscal_period_id INT NOT NULL,
    action VARCHAR(20) NOT NULL,
    performed_by_user_id INT NOT NULL,
    performed_at DATETIME NOT NULL,
    trace_id VARCHAR(50) NULL,
    FOREIGN KEY (fiscal_period_id) REFERENCES fiscal_periods(id)
);
```

### 8.2 Cambios Requeridos
1. Crear entidades de dominio `FiscalPeriod` y `FiscalPeriodAuditLog`
2. Agregar DbSets al `ConsulconDbContext`
3. Modificar `FiscalPeriodService` para usar EF Core
4. Eliminar `InMemoryFiscalPeriodStore`

---

## 9. Testing

### 9.1 Tests Manuales Realizados
- ✅ Cerrar período exitosamente
- ✅ Verificar bloqueo de escritura en período cerrado
- ✅ Consultar períodos cerrados
- ✅ Consultar log de auditoría

### 9.2 Tests Unitarios Recomendados
- `FiscalPeriodService_ClosePeriod_ShouldSucceed`
- `FiscalPeriodService_ClosePeriod_AlreadyClosed_ShouldFail`
- `FiscalPeriodService_ClosePeriod_FuturePeriod_ShouldFail`
- `PeriodLockMiddleware_BlocksWriteOnClosedPeriod`

---

## 10. Archivos Modificados (No Relacionados)

Durante la implementación se detectó y corrigió un error preexistente:

**Archivo:** `tests/Consulcon.IntegrationTests/ConsulconWebApplicationFactory.cs`  
**Problema:** Mock `NoOpTenantDatabaseService` no implementaba `GetCondominioAsync`  
**Solución:** Se agregó el método faltante

---

## Apéndice A: Estructura de Archivos

```
src/
├── Consulcon.API/
│   ├── Controllers/Contabilidad/
│   │   └── FiscalPeriodsController.cs        [NUEVO]
│   ├── Middleware/
│   │   ├── ExceptionMiddleware.cs
│   │   └── PeriodLockMiddleware.cs           [NUEVO]
│   └── Program.cs                            [MODIFICADO]
│
├── Consulcon.Application/
│   ├── DTOs/Contabilidad/FiscalPeriods/
│   │   ├── ClosePeriodRequest.cs             [NUEVO]
│   │   └── FiscalPeriodDto.cs                [NUEVO]
│   └── Interfaces/Contabilidad/
│       └── IFiscalPeriodService.cs           [NUEVO]
│
└── Consulcon.Infrastructure/
    ├── DependencyInjection.cs                [MODIFICADO]
    └── Services/Contabilidad/
        ├── InMemoryFiscalPeriodStore.cs      [NUEVO]
        └── FiscalPeriodService.cs            [NUEVO]

docs/99 - Otros/02 - Postman/
└── postman_collection.json                   [MODIFICADO]

tests/Consulcon.IntegrationTests/
└── ConsulconWebApplicationFactory.cs         [MODIFICADO - Fix preexistente]
```
