# 13. Balance Histórico Proyectado y Snapshot Job

**Sprint:** 3  
**Tipo:** Addition  
**Fecha:** 07/02/2026

## Visión General

Esta funcionalidad permite a los administradores **visualizar el historial de saldos** de cada cuenta financiera (Bancos y Caja Chica) de manera independiente y eficiente.

Se implementa un **Job automático de snapshots diarios** que captura el saldo al cierre de cada día, junto con una **lógica de reconstrucción en tiempo real** que proyecta el saldo actual combinando el último snapshot persistido con los movimientos vivos del día en curso.

Incluye soporte nativo para **Multi-tenancy**, garantizando que los saldos históricos y proyectados se gestionen de forma aislada por condominio (Tenant).

## Nuevas Entidades

### 1. `AccountDailyBalance` (Saldos Diarios de Cuenta)

Ubicación: `src/Consulcon.Domain/Entities/Contabilidad/AccountDailyBalance.cs`

- **Propósito**: Almacenar el rastro histórico del saldo final de una cuenta financiera por día, optimizando las consultas de reportes y evitando cálculos retrospectivos costosos.
- **Relación DB**:  
  Reside en la **base de datos del Tenant**.  
  Mantiene una relación **muchos a uno** con:
  - `Banco` (cuenta financiera)
- **Propiedades Clave**:
  - `Id`: Identificador único del registro.
  - `IdBanco`: Referencia a la cuenta financiera.
  - `Balance`: Monto exacto del saldo al momento del snapshot.
  - `Date`: Fecha de captura del saldo (generalmente a medianoche).

## Nuevos DTOs

Ubicación: `src/Consulcon.Application/DTOs/Contabilidad/`

- `BalanceHistoryDto`:  
  DTO de lectura utilizado para exponer el historial de saldos.  
  Incluye:
  - Fecha del registro.
  - Saldo capturado o proyectado.
  - Nombre de la entidad financiera, facilitando la visualización en reportes y dashboards.

## Nuevo Controller

Ubicación: `src/Consulcon.API/Controllers/AccountsController.cs`

- Ruta Base: `api/Accounts`

### Endpoints

#### Historial Financiero

### 2. Obtener Historial de Transacciones (Estado de Cuenta)

- `GET /api/Accounts/{id}/balance-history`:  
  Obtiene el historial de transacciones (Egresos) de una cuenta específica dentro de un rango de fechas.
  - Retorna una lista detallada con: Fecha, Concepto, Monto, Beneficiario, Tipo de Transacción, etc.
  - Actúa como un **Estado de Cuenta** filtrado por fechas.

## Servicios

Ubicación: `src/Consulcon.Infrastructure/Services/`

### 1. `AccountSnapshotBackgroundService` (IHostedService)

- **Funcionalidad**:
  - Actúa como un **Worker nocturno** que se ejecuta cada 24 horas.
  - Crea un **Scope de base de datos** para leer los saldos actuales de todas las cuentas financieras registradas.
  - Genera y persiste los snapshots diarios en la tabla histórica.
- **Interacción DB**:
  - Acceso directo a `ConsulconDbContext`.
  - Lectura masiva de cuentas financieras.
  - Inserción por lotes de registros `AccountDailyBalance`.

### 2. `AccountService` (Implementación de `IAccountService`)

- **Funcionalidad**:
  - Gestiona la **lógica de reconstrucción de saldos**.
  - Combina los datos históricos de `AccountDailyBalance` con el saldo vivo de la tabla `Bancos` cuando la consulta requiere información del día en curso.
- **Interacción DB**:
  - Consultas filtradas por **rango de fechas** y **ID de cuenta**.
  - Lectura conjunta del historial persistido y del estado actual de la cuenta bancaria.

## Postman Collection

**Archivo:** `docs/99 - Otros/02-postman/postman_collection.json`  
**Carpeta:** `Tesoreria > Bancos`

### Request 1: Get Balance History (Projected)

| Campo           | Valor                                                                      |
| --------------- | -------------------------------------------------------------------------- |
| **Nombre**      | Get Balance History (Projected)                                            |
| **Método**      | `GET`                                                                      |
| **URL**         | `{{baseUrl}}/api/Accounts/{{accountId}}/balance-history`                   |
| **Headers**     | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}` |
| **Query**       | `from` (date, opcional), `to` (date, opcional)                             |
| **Descripción** | Obtiene la lista de transacciones (Egresos) en el rango de fechas.         |

### Request 2: Get All Accounts

| Campo       | Valor                                                                      |
| ----------- | -------------------------------------------------------------------------- |
| **Nombre**  | Get All Accounts                                                           |
| **Método**  | `GET`                                                                      |
| **URL**     | `{{baseUrl}}/api/Accounts?activeOnly=true`                                 |
| **Headers** | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}` |
| **Query**   | `activeOnly` (bool, default: true)                                         |

### Request 3: Create Account

| Campo       | Valor                                                                                                        |
| ----------- | ------------------------------------------------------------------------------------------------------------ |
| **Nombre**  | Create Account                                                                                               |
| **Método**  | `POST`                                                                                                       |
| **URL**     | `{{baseUrl}}/api/Accounts`                                                                                   |
| **Headers** | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}`                                   |
| **Body**    | JSON: `{ "name": "Banco X", "accountNumber": "123", "type": "BANCO", "isActive": true, "balance": 1000.00 }` |

### Request 4: Update Account

| Campo       | Valor                                                                      |
| ----------- | -------------------------------------------------------------------------- |
| **Nombre**  | Update Account                                                             |
| **Método**  | `PUT`                                                                      |
| **URL**     | `{{baseUrl}}/api/Accounts/{{accountId}}`                                   |
| **Headers** | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}` |
| **Body**    | JSON: `AccountDto`                                                         |

### Request 5: Delete Account

| Campo       | Valor                                                                      |
| ----------- | -------------------------------------------------------------------------- |
| **Nombre**  | Delete Account                                                             |
| **Método**  | `DELETE`                                                                   |
| **URL**     | `{{baseUrl}}/api/Accounts/{{accountId}}`                                   |
| **Headers** | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}` |
