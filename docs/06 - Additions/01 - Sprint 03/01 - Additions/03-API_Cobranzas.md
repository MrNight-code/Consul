# 03. API de Registro y Gestión de Cobranzas

**Sprint:** 03
**Tipo:** Addition
**Fecha:** 27/01/2026

## Visión General

Esta API permite al Administrador registrar los pagos (cobranzas) realizados por las unidades habitacionales. Es el **núcleo de la entrada de capital** del sistema.

Implementa lógica financiera crítica:

1.  **Imputación FIFO**: Los pagos se aplican automáticamente a las deudas más antiguas.
2.  **Saldo Transaccional**: Actualiza el saldo deudor de la unidad en tiempo real dentro de una transacción segura.
3.  **Auditoría**: Genera registros de transacciones inalterables (`TransaccionPago`).

> [!NOTE]
> Esta implementación utiliza las tablas existentes `TransaccionPagos`, `Propiedad`, `Contrato`, y `DeudaCabecera` para mantener la integridad con el sistema legado.

## Entidades Principales

### `TransaccionPago` (Registro de Cobro)

Ubicación: `src/Consulcon.Domain/Entities/Facturacion/TransaccionPago.cs`

- **Propósito**: Registro inmutable de un pago aplicado a una deuda específica.
- **Relación**: Vincula una `DeudaCabecera` con un `IdPersonaPagador` (Titular) y un `IdBancoDestino` (Cuenta receptora).

#### Propiedades Clave:

| Propiedad          | Tipo     | Descripción                                     |
| :----------------- | :------- | :---------------------------------------------- |
| `IdPago`           | int      | Identificador único (Auto-incremental)          |
| `IdDeuda`          | int      | Deuda a la que se aplicó el pago                |
| `IdPersonaPagador` | int      | ID de la persona responsable del pago (Titular) |
| `IdBancoDestino`   | int      | ID de la cuenta/banco donde ingresó el dinero   |
| `MontoAbonado`     | decimal  | Monto aplicado a esta deuda específica          |
| `FechaPago`        | DateTime | Fecha y hora del servidor (Auditoría)           |
| `Estado`           | string   | "CONFIRMADO"                                    |

## DTO

Ubicación: `src/Consulcon.Application/DTOs/CobranzaRequest.cs`

```csharp
public class CobranzaRequest
{
    public int UnitId { get; set; }           // ID de la Propiedad/Unidad
    public decimal Monto { get; set; }        // Monto total pagado
    public int IdFormaPago { get; set; }      // Método de pago (Efectivo, Transferencia...)
    public string? NroReferencia { get; set; }// Nro de Comprobante / Transacción
    public string? Observaciones { get; set; }
    public int? IdBancoDestino { get; set; }  // Cuenta destino (Requerido)
}
```

## Controller

Ubicación: `src/Consulcon.API/Controllers/CobranzasController.cs`

- Ruta Base: `api/cobranzas`

### Endpoints

#### Registrar Cobranza

| Método | Ruta             | Descripción                                                         |
| :----- | :--------------- | :------------------------------------------------------------------ |
| `POST` | `/api/cobranzas` | Registra un nuevo pago y lo distribuye entre las deudas pendientes. |

**Validaciones:**

- El monto debe ser > 0.
- La Unidad (`UnitId`) debe existir.
- Debe existir un Contrato Activo con un Titular para la unidad (para asignar el pagador).
- La cuenta destino (`IdBancoDestino`) debe existir y estar activa.

#### Consultar Historial

| Método | Ruta                      | Descripción                                  |
| :----- | :------------------------ | :------------------------------------------- |
| `GET`  | `/api/cobranzas/{unitId}` | Obtiene el historial de pagos de una unidad. |

### Ejemplos de Request/Response

#### POST /api/cobranzas

**Request Body:**

```json
{
  "unitId": 105,
  "monto": 500.0,
  "idFormaPago": 2, // Transferencia
  "nroReferencia": "TRX-998877",
  "observaciones": "Pago expensas Marzo",
  "idBancoDestino": 1 // Banco Nacional
}
```

**Response (200 OK):**

```json
{
  "isSuccess": true,
  "message": "Cobranza registrada exitosamente."
}
```

**Response (Error - Sin Titular):**

```json
{
  "isSuccess": false,
  "error": "No se encontró un titular/pagador asociado a la unidad para registrar el cobro."
}
```

## Lógica de Negocio (Service Layer)

Ubicación: `src/Consulcon.Infrastructure/Services/CobranzaService.cs`

El servicio ejecuta la siguiente secuencia dentro de una **Transacción de Base de Datos**:

1.  **Validación**: Verifica unidad, monto y cuenta destino.
2.  **Resolución de Pagador**: Busca el contrato vigente de la unidad y obtiene el `IdPersona` del participante con rol "Titular".
3.  **Actualización de Saldo Global**: Descuenta el monto total del `SaldoDeudor` en la tabla `Propiedad`.
4.  **Distribución FIFO**:
    - Obtiene todas las deudas pendientes (`DeudaCabecera`) de la unidad, ordenadas por fecha de vencimiento ascendente.
    - Recorre las deudas aplicando el saldo disponible.
    - Crea un registro `TransaccionPago` por cada deuda afectada indicando cuánto se abonó a esa deuda específica.
    - Actualiza el estado de la deuda a `PARCIAL` o `PAGADO`.

## Tests

Ubicación: `tests/Consulcon.IntegrationTests/Services/CobranzaServiceTests.cs`

| Test                                                | Descripción                                                                                    |
| :-------------------------------------------------- | :--------------------------------------------------------------------------------------------- |
| `RegistrarCobranza_ShouldUpdateBalanceAndApplyFIFO` | Verifica que un pago único cubra múltiples deudas antiguas y actualice el saldo correctamente. |

```bash
dotnet test tests/Consulcon.IntegrationTests/Consulcon.IntegrationTests.csproj --filter "FullyQualifiedName~CobranzaServiceTests"
```

## Arquitectura y Relaciones

```mermaid
graph TD
    API[CobranzasController] --> Service[CobranzaService]
    Service --> DB[(ConsulconDbContext)]

    subgraph Lógica de Negocio
    Service --> Prop[Propiedad]
    Prop --> Cont[Contrato]
    Cont --> Part[ContratoParticipante]
    Part --> Pers[Persona (Pagador)]

    Service --> Deuda[DeudaCabecera]
    Service --> Pago[TransaccionPago]
    Service --> Banco[Banco (Destino)]
    end

    Pago --> Deuda
    Pago --> Pers
    Pago --> Banco
```

## Criterios de Aceptación ✅

| Criterio                           | Estado | Notas                                               |
| :--------------------------------- | :----- | :-------------------------------------------------- |
| CRUD de registros (Create/Read)    | ✅     | Update/Delete omitidos por inmutabilidad.           |
| Actualización automática de saldo  | ✅     | Verificado en tests.                                |
| Lógica FIFO                        | ✅     | Verificado en tests.                                |
| Vinculación Correcta (Tenant/Unit) | ✅     | Filtrado por `UnitId` asegura aislamiento.          |
| Sin Hardcoding                     | ✅     | `IdPersona` y `IdBanco` se resuelven dinámicamente. |
