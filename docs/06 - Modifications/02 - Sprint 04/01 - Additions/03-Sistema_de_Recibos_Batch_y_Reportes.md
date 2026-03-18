# 03. Sistema de Recibos Batch y Reportes PDF

**Sprint:** 04
**Tipo:** Addition
**Fecha:** 11/03/2026
**Módulo:** Facturación - Recibos

## Visión General

Se implementó un sistema completo para la **generación masiva de recibos en formato PDF** y su descarga en lotes. El sistema permite generar un único documento PDF con múltiples recibos filtrados por rango de fechas y unidad, así como descargar recibos existentes empaquetados en un archivo ZIP. Adicionalmente, se migró la lógica de recibos desde `CobranzaController` hacia `PagoController` y se creó un nuevo controlador dedicado a reportes.

## Nuevas Entidades / DTOs

### 1. `BatchReceiptRequestDto`

Ubicación: `src/Consulcon.Application/DTOs/Facturacion/BatchReceiptRequestDto.cs`

- **Propósito**: Request para generar recibos en lote.
- **Propiedades Clave**:
  - `StartDate`: Fecha de inicio del rango.
  - `EndDate`: Fecha de fin del rango.
  - `UnitId` (opcional): Filtro por unidad/propiedad específica.

### 2. `ReceiptDto` (Modificado)

Ubicación: `src/Consulcon.Application/DTOs/Facturacion/ReceiptDto.cs`

| Campo           | Descripción                                      |
| --------------- | ------------------------------------------------ |
| `IdPago`        | Identificador de la transacción                  |
| `ReciboUrl`     | Ruta del archivo PDF generado                    |
| `FechaRecibo`   | Fecha de emisión del recibo                      |
| `MontoAbonado`  | Monto total abonado                              |
| `NombrePersona` | Nombre del pagador                               |
| `MetodoPago`    | Método de pago utilizado                         |
| `CodigoUnidad`  | Código de la unidad asociada                     |

### 3. `ReceiptWithFiltersSpec` (Specification)

Ubicación: `src/Consulcon.Domain/Specifications/ReceiptWithFiltersSpec.cs`

- **Propósito**: Implementación del patrón Specification para filtrar recibos por medio de pago y propiedad, con paginación integrada.
- **Filtros Soportados**: `medio` (tipo forma de pago), `propiedadId` (unidad).

## Nuevo Controller

### `ReceiptReportsController`

Ubicación: `src/Consulcon.API/Controllers/Facturacion/ReceiptReportsController.cs`

- Ruta Base: `api/reports`
- Requiere autenticación (`[Authorize]`).

#### 1. Generar Recibos Batch (PDF)

| Propiedad       | Valor                                                              |
| --------------- | ------------------------------------------------------------------ |
| **Método**      | `POST`                                                             |
| **Ruta**        | `api/reports/receipts-batch`                                       |
| **Descripción** | Genera un PDF multi-página con todos los recibos del rango de fechas |
| **Body (JSON)** | `{ "startDate": "2026-01-01", "endDate": "2026-03-01", "unitId": 1 }` |
| **Respuesta**   | Archivo PDF descargable (`application/pdf`)                        |

### Endpoints en `PagoController` (Migrados desde CobranzaController)

Ubicación: `src/Consulcon.API/Controllers/Facturacion/PagoController.cs`

#### 2. Listar Recibos Generados (Paginado)

| Propiedad       | Valor                                                                  |
| --------------- | ---------------------------------------------------------------------- |
| **Método**      | `GET`                                                                  |
| **Ruta**        | `api/pago/recibos?pageNumber=1&pageSize=10&medio=Transferencia&propiedadId=1` |
| **Descripción** | Lista recibos generados con filtros y paginación via Specification Pattern |
| **Respuesta**   | `PagedResult<ReceiptDto>`                                              |

#### 3. Descargar Recibo Individual

| Propiedad       | Valor                                     |
| --------------- | ----------------------------------------- |
| **Método**      | `GET`                                     |
| **Ruta**        | `api/pago/recibos/{filename}`             |
| **Descripción** | Descarga un archivo PDF de recibo por nombre |
| **Respuesta**   | Archivo PDF (`application/pdf`)           |

#### 4. Descargar Recibos por Mes (ZIP)

| Propiedad       | Valor                                                   |
| --------------- | ------------------------------------------------------- |
| **Método**      | `GET`                                                   |
| **Ruta**        | `api/pago/recibos/batch?mes=3&anio=2026`                |
| **Descripción** | Descarga un ZIP con todos los recibos generados de un mes |
| **Respuesta**   | Archivo ZIP (`application/zip`)                         |

## Servicios

### `ReceiptGenerationService` (Actualizado)

Ubicación: `src/Consulcon.Infrastructure/Services/Facturacion/ReceiptGenerationService.cs`

- Implementa `IReceiptGenerationService`.
- **Nuevos Métodos**:
  - `GetGeneratedReceiptsAsync(PaginationParams, medio?, propiedadId?)` — Consulta paginada con Specification Pattern.
  - `GetBatchZipAsync(int mes, int anio)` — Empaqueta recibos existentes en ZIP.
  - `GenerateBatchReceiptsPdfAsync(BatchReceiptRequestDto)` — Genera PDF multi-página con QuestPDF.
- **Motor PDF**: Usa la librería `QuestPDF` con licencia Community.
- **Diseño del Recibo**: Formato A4, incluye tabla de conceptos de deuda, datos del pagador, banco, método de pago y firma digital temporal.

## Impacto

- Los endpoints de recibos fueron **migrados** de `CobranzaController` a `PagoController`, centralizando toda la lógica de pagos y recibos en un solo controlador.
- `CobranzaController` quedó simplificado con solo 3 endpoints: registrar cobranza, obtener historial, y listado paginado.
- El nuevo `ReceiptReportsController` está separado para reportes administrativos que requieren lógica diferente (tenant validation explícita, logging, etc.).
