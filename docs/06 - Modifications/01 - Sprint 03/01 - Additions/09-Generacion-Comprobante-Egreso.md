# 09. Generación de Comprobante de Egreso

**Sprint:** 03  
**Tipo:** Addition  
**Fecha:** 05/02/2026

---

## Visión General

Implementación de la generación de comprobantes PDF para los egresos registrados en el sistema. Permite a los administradores descargar un respaldo físico de los pagos realizados a proveedores. El documento incluye un código QR para validación rápida y firma digital temporal.

---

## Nuevas Entidades

No se crean nuevas entidades. Se reutilizan:

- `Egreso` (Gasto)
- `Proveedor` (Supplier)

---

## Nuevos DTOs

No se requieren DTOs de entrada. La salida es un archivo binario (`FileStreamResult`).

---

## Controller

**Ubicación:** `src/Consulcon.API/Controllers/Contabilidad/TesoreriaController.cs`

---

## Endpoints

### 1. Generar Comprobante PDF

| Propiedad       | Valor                                                                                                                                     |
| --------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| **Método**      | `GET`                                                                                                                                     |
| **Ruta**        | `/api/tesoreria/egresos/{id}/comprobante`                                                                                                 |
| **Descripción** | Genera y descarga un comprobante PDF del egreso especificado. Incluye datos del proveedor, concepto, monto, QR de validación y timestamp. |
| **Parámetros**  | `id` (int): ID del egreso.                                                                                                                |
| **Respuesta**   | Archivo PDF (`application/pdf`) con nombre `Egreso_{id}_{Fecha}.pdf`.                                                                     |

---

## Servicios

**Ubicación:** `src/Consulcon.Infrastructure/Services/Facturacion/ExpenseReceiptGenerationService.cs`

- Implementa `IExpenseReceiptGenerationService`.
- Obtiene datos del `Egreso` con sus relaciones (`Proveedor`, `Banco`, `FormaPago`).
- Genera PDF usando `QuestPDF`:
  - Header: Condominio y proveedor.
  - Body: Tabla con concepto y monto.
  - Footer: Timestamp, hash (firma) y QR.
- Genera código QR usando `QRCoder` (incrustado en PDF).
- No guarda archivos en disco, solo en memoria (`MemoryStream`).

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02-postman/postman_collection.json`  
**Carpeta:** `Otros > Egreso`

### Request 1: Generar Comprobante (PDF)

| Campo                  | Valor                                                              |
| ---------------------- | ------------------------------------------------------------------ |
| **Nombre**             | Generar Comprobante (PDF)                                          |
| **Método**             | `GET`                                                              |
| **URL**                | `{{baseUrl}}/api/Tesoreria/egresos/:id/comprobante`                |
| **Headers**            | `Authorization: Bearer {{authToken}}`, `X-Tenant-Id: {{tenantId}}` |
| **Parámetros de Ruta** | `id`: ID del egreso (ej: `1`)                                      |
| **Body**               | N/A (GET request)                                                  |
