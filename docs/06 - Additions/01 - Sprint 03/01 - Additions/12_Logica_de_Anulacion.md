# 12. Anulación de Gastos

**Sprint:** 3  
**Tipo:** Addition  
**Fecha:** 06/02/2026

## Visión General

Esta funcionalidad permite a los administradores **anular egresos registrados** de manera segura y auditable.  
Su objetivo principal es **preservar la integridad financiera**, ejecutando la **reversión automática del saldo** en la cuenta bancaria de origen, sin eliminar registros contables.

El proceso mantiene un **rastro de auditoría inmutable** mediante **Snapshots JSON**, cumple normas de auditoría interna y soporta **Multi-tenancy**, asegurando que cada operación se ejecute de forma aislada por condominio (Tenant).

## Nuevas Entidades

### 1. `Egreso` (Gasto / Egreso de Caja o Banco)

Ubicación: `src/Consulcon.Domain/Entities/Contabilidad/Egreso.cs`

- **Propósito**: Representa la salida física o electrónica de dinero utilizada para el pago de obligaciones del condominio.
- **Relación DB**:  
  Reside en la **base de datos del Tenant**.  
  Mantiene una relación **muchos a uno** con:
  - `Banco` (cuenta bancaria de origen)
  - `Proveedor`
- **Propiedades Clave**:
  - `IdEgreso`: Identificador único (Primary Key).
  - `Estado`: Define el ciclo de vida del egreso (ej. `VOIDED`).
  - `IdBancoOrigen`: Referencia a la cuenta bancaria utilizada.
  - `MontoTotal`: Monto total del egreso, utilizado para la reversión del saldo.

## Nuevos DTOs

Ubicación: `src/Consulcon.Application/DTOs/Contabilidad/`

- `VoidExpenseRequest`:  
  DTO utilizado para capturar la solicitud de anulación de un egreso.
  - **Validaciones**:
    - `Reason`: obligatorio, con longitud mínima de **10 caracteres**.

## Nuevo Controller

Ubicación: `src/Consulcon.API/Controllers/Contabilidad/ContabilidadController.cs`

- Ruta Base: `api/Contabilidad`

### Endpoints

#### Gestión de Egresos

- `POST /api/expenses/{id}/void`:  
  Recibe el identificador del egreso y el motivo de anulación.  
  Ejecuta la reversión del saldo bancario, registra el snapshot de auditoría y actualiza el estado del egreso a **anulado**.

## Servicios

Ubicación: `src/Consulcon.Application/Services/Contabilidad/ContabilidadService.cs`

- Implementa `IContabilidadService`.
- **Funcionalidad**:
  - Valida la existencia del egreso.
  - Verifica que el período contable esté abierto.
  - Genera un **Snapshot JSON** del estado original del egreso utilizando `System.Text.Json`.
  - Orquesta la reversión del saldo en la cuenta bancaria.
  - Marca el egreso como **VOIDED** sin eliminación física.
- **Interacción DB**:
  - Utiliza `IUnitOfWork` para garantizar **atomicidad transaccional**.
  - Accede a los datos mediante:
    - `IRepository<Egreso>`
    - `IRepository<Banco>`
  - La reversión del saldo y la anulación del egreso se confirman en una única transacción.

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02-postman/postman_collection.json`  
**Carpeta:** `Entities > Egreso`

### Request 1: Void Expense

| Campo                  | Valor                                                              |
| ---------------------- | ------------------------------------------------------------------ |
| **Nombre**             | Void Expense                                                       |
| **Método**             | `POST`                                                             |
| **URL**                | `{{baseUrl}}/api/expenses/:id/void`                                |
| **Headers**            | `Authorization: Bearer {{authToken}}`, `X-Tenant-Id: {{tenantId}}` |
| **Parámetros de Ruta** | `id`: ID del egreso (ej: `1`)                                      |
| **Body (JSON)**        | `{ "reason": "Anulación por error en registro..." }`               |
