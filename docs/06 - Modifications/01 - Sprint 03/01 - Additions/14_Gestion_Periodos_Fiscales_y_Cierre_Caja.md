# 14. Gestión de Periodos Fiscales y Cierre de Caja

**Sprint:** 3  
**Tipo:** Addition  
**Fecha:** 11/02/2026

## Visión General

Esta funcionalidad introduce un control estricto sobre los periodos contables y la gestión de efectivo (Caja Chica / Bancos), permitiendo:

1.  **Cierre de Periodos Fiscales**: Bloquear operaciones (escritura/edición/borrado) en rangos de fechas específicos para preservar la integridad contable una vez presentados los impuestos o informes.
2.  **Middleware de Bloqueo (PeriodLock)**: Intercepta todas las solicitudes `POST`, `PUT`, `DELETE` y verifica si la fecha de la operación cae dentro de un periodo cerrado.
3.  **Cierre de Caja (Arqueo)**: Permite realizar cortes diarios o mensuales de caja, congelando saldos y movimientos.

## Nuevas Entidades

### 1. `FiscalPeriod` (Periodo Fiscal)

- **Ubicación**: `src/Consulcon.Domain/Entities/Contabilidad/FiscalPeriod.cs`
- **Propósito**: Definir un rango de fechas (`StartDate` - `EndDate`) que está cerrado contablemente.
- **Relación**: Por `Tenant` (Condominio).
- **Propiedades**:
  - `IsClosed`: Indica si el periodo está cerrado.
  - `LockedAt`: Fecha de cierre.

## Middleware

### `PeriodLockMiddleware`

- **Ubicación**: `src/Consulcon.API/Middleware/PeriodLockMiddleware.cs`
- **Lógica**:
  - Intercepta métodos mutables (`POST`, `PUT`, `DELETE`, `PATCH`).
  - Extrae fechas relevantes del cuerpo de la solicitud (e.g., `FechaContable`, `FechaPago`).
  - Consulta `IFiscalPeriodService` (optimizado con caché en memoria) para verificar si la fecha está en un periodo cerrado.
  - **Retorno**: `403 Forbidden` si el periodo está cerrado.

## Servicios

### `FiscalPeriodService` & `InMemoryFiscalPeriodStore`

- **Ubicación**: `src/Consulcon.Infrastructure/Services/Contabilidad/`
- **Funcionalidad**:
  - Gestión CRUD de periodos fiscales.
  - `InMemoryFiscalPeriodStore`: Mantiene una copia en caché de los periodos cerrados para evitar consultas a BD en cada request del Middleware.

## Endpoints (API)

### API Endpoints

Los endpoints se encuentran en `api/fiscal-periods`. **Importante:** Todos los endpoints requieren el header `X-Condominio-Id` (entero) para identificar el contexto. El anterior header `X-Tenant-Id` ha sido depurado.

| Acción                    | Método | Endpoint                                    | Body (JSON)                  |
| :------------------------ | :----- | :------------------------------------------ | :--------------------------- |
| **Cerrar Período**        | `POST` | `/api/fiscal-periods/close`                 | `{"year": 2026, "month": 1}` |
| **Reabrir Período**       | `POST` | `/api/fiscal-periods/reopen`                | `{"year": 2026, "month": 1}` |
| **Ver Períodos Cerrados** | `GET`  | `/api/fiscal-periods`                       | -                            |
| **Verificar Estado**      | `GET`  | `/api/fiscal-periods/status/{year}/{month}` | -                            |
| **Log de Auditoría**      | `GET`  | `/api/fiscal-periods/audit-log`             | -                            |

> [!TIP]
> El `CondominioId` ya no se envía en el cuerpo de la petición (body) para el cierre, sino que se infiere automáticamente del header `X-Condominio-Id`.

### Colección Postman

La carpeta `FiscalPeriods` en la colección principal ha sido actualizada:

1.  **Close Fiscal Period**: Actualizado para usar solo `X-Condominio-Id` y remover el ID del body.
2.  **Reopen Fiscal Period**: Nuevo request agregado.
3.  **Audit Log / Status**: Actualizados para no requerir el ID en la ruta (se usa el header).

**Carpeta:** `FiscalPeriods` (Nueva carpeta en la raíz o bajo Contabilidad)

### Request 1: Close Fiscal Period

- **Body**:
  ```json
  {
    "startDate": "2024-01-01",
    "endDate": "2024-01-31",
    "notes": "Cierre Enero 2024"
  }
  ```

### Request 2: Check Date

- **URL**: `{{baseUrl}}/api/FiscalPeriods/check-date/2024-01-15`
- **Respuesta esperada**: `true` (si está cerrado) o `false`.
