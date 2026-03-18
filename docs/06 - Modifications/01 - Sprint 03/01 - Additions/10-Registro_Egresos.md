# 10. Registro de Egresos y Transacción Financiera

**Sprint:** 03  
**Tipo:** Addition  
**Fecha:** 05/02/2026

---

## Visión General

Esta funcionalidad permite a los administradores registrar cada gasto o factura pagada por el condominio, asociándola a una categoría y a una cuenta de salida. La operación es atómica: el registro del gasto y el descuento del saldo de la cuenta ocurren simultáneamente o no ocurren en absoluto.

---

## Nuevas Entidades

### `AccountTransactionHistory`

| Campo         | Tipo       | Descripción                                      |
| ------------- | ---------- | ------------------------------------------------ |
| `Id`          | `Guid`     | Identificador único.                             |
| `AccountId`   | `int`      | FK a `Banco` (cuenta origen).                    |
| `ExpenseId`   | `int?`     | FK a `Egreso` (opcional).                        |
| `Amount`      | `decimal`  | Monto de la transacción (negativo para egresos). |
| `Date`        | `DateTime` | Fecha de la transacción.                         |
| `Description` | `string`   | Descripción del movimiento.                      |
| `ReferenceId` | `string`   | Referencia cruzada (ej: ID del Egreso).          |

**Ubicación:** `src/Consulcon.Domain/Entities/Contabilidad/AccountTransactionHistory.cs`

### `Egreso` (Existente - Modificado)

Se utiliza la entidad existente para el registro del gasto.

**Ubicación:** `src/Consulcon.Domain/Entities/Contabilidad/Egreso.cs`

---

## Nuevos DTOs

### `RegisterExpenseCommand`

| Campo             | Tipo       | Descripción                             |
| ----------------- | ---------- | --------------------------------------- |
| `CondominioId`    | `int`      | ID del condominio.                      |
| `AccountId`       | `int`      | FK a `Banco` (cuenta origen de fondos). |
| `Amount`          | `decimal`  | Monto del gasto.                        |
| `Description`     | `string`   | Concepto del gasto.                     |
| `ExpenseDate`     | `DateTime` | Fecha del gasto.                        |
| `CategoryId`      | `int`      | FK a `AutorizacionGasto`.               |
| `PaymentMethodId` | `int`      | FK a `FormaPago`.                       |
| `ProviderId`      | `int?`     | FK a `Proveedor` (opcional).            |
| `InvoiceNumber`   | `string?`  | Número de factura (opcional).           |

**Ubicación:** `src/Consulcon.Application/DTOs/Contabilidad/Expenses/RegisterExpenseCommand.cs`

---

## Controller

**Ubicación:** `src/Consulcon.API/Controllers/Contabilidad/ExpensesController.cs`  
**Ruta Base:** `api/expenses`

---

## Endpoints

### 1. Registrar Gasto (Register Expense)

| Propiedad        | Valor                                                                                                                                        |
| ---------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| **Método**       | `POST`                                                                                                                                       |
| **Ruta**         | `/api/expenses`                                                                                                                              |
| **Descripción**  | Registra un nuevo egreso y actualiza el saldo de la cuenta asociada de forma transaccional. Crea un registro en `AccountTransactionHistory`. |
| **Validaciones** | `ExpenseDate` <= `DateTime.UtcNow`. `Amount` > 0.01. Saldo suficiente (opcional).                                                            |
| **Body**         | `RegisterExpenseCommand` (JSON).                                                                                                             |
| **Respuesta**    | `Result<int>` con el ID del nuevo egreso (`IdEgreso`).                                                                                       |

---

## Servicios

**Ubicación:** `src/Consulcon.Infrastructure/Services/Contabilidad/ExpenseService.cs`

- Implementa `IExpenseService`.
- **Flujo Transaccional:**
  1. Inicio de transacción (`BeginTransactionAsync`).
  2. Obtención de cuenta (`Banco`) y validación de saldo.
  3. Descuento de saldo: `banco.Debit(cmd.Amount)`.
  4. Creación de `Egreso`.
  5. Registro en `AccountTransactionHistory`.
  6. Commit de transacción.

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02-postman/postman_collection.json`  
**Carpeta:** `Otros > Egreso`

### Request 1: Register Expense (Transactional)

| Campo           | Valor                                                                                                |
| --------------- | ---------------------------------------------------------------------------------------------------- |
| **Nombre**      | Register Expense (Transactional)                                                                     |
| **Método**      | `POST`                                                                                               |
| **URL**         | `{{baseUrl}}/api/expenses`                                                                           |
| **Headers**     | `Authorization: Bearer {{authToken}}`, `X-Tenant-Id: {{tenantId}}`, `Content-Type: application/json` |
| **Body (JSON)** | Ver ejemplo abajo.                                                                                   |

**Ejemplo Body:**

```json
{
  "condominioId": 1,
  "accountId": 1,
  "amount": 150.0,
  "description": "Compra de insumos reserva",
  "expenseDate": "2026-02-05T14:30:00Z",
  "categoryId": 1,
  "paymentMethodId": 1,
  "providerId": null,
  "invoiceNumber": "F-999"
}
```
