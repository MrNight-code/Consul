# 08. Adjuntos de Gastos (Facturas)

**Sprint:** 03  
**Tipo:** Addition  
**Fecha:** 05/02/2026

---

## Visión General

Esta funcionalidad permite a los administradores adjuntar una fotografía o archivo PDF de la factura física a un registro de egreso (Gasto/Expense). El objetivo es tener un respaldo digital centralizado y auditable de cada gasto. Incluye validación de seguridad para asegurar que solo usuarios del Tenant correspondiente puedan acceder a los archivos.

---

## Nuevas Entidades

### `ExpenseAttachment`

| Campo            | Tipo       | Descripción                               |
| ---------------- | ---------- | ----------------------------------------- |
| `Id`             | `Guid`     | Identificador único del adjunto.          |
| `ExpenseId`      | `int`      | FK al Egreso asociado.                    |
| `FileName`       | `string`   | Nombre original del archivo (sanitizado). |
| `StoredFileName` | `string`   | Nombre en disco (Guid + extensión).       |
| `ContentType`    | `string`   | Tipo MIME (image/jpeg, application/pdf).  |
| `Size`           | `long`     | Tamaño en bytes.                          |
| `StoragePath`    | `string`   | Ruta relativa de almacenamiento.          |
| `UploadedAt`     | `DateTime` | Fecha de subida.                          |
| `UploadedBy`     | `string`   | Usuario que subió el archivo.             |

**Ubicación:** `src/Consulcon.Domain/Entities/Inmuebles/ExpenseAttachment.cs`

---

## Nuevos DTOs

| DTO                    | Propósito                                       |
| ---------------------- | ----------------------------------------------- |
| `ExpenseAttachmentDto` | Lectura/respuesta. Contiene Id, FileName, Size. |
| `UploadAttachmentDto`  | Subida. Contiene `IFormFile File`.              |

**Ubicación:** `src/Consulcon.Application/DTOs/Inmuebles/`

---

## Controller

**Ubicación:** `src/Consulcon.API/Controllers/Inmuebles/ExpensesController.cs` y `AttachmentsController.cs`

---

## Endpoints

### 1. Subir Adjunto (Upload Attachment)

| Propiedad        | Valor                                                                         |
| ---------------- | ----------------------------------------------------------------------------- |
| **Método**       | `POST`                                                                        |
| **Ruta**         | `/api/expenses/{id}/attachments`                                              |
| **Descripción**  | Sube un archivo (imagen o PDF de factura) y lo asocia al egreso especificado. |
| **Parámetros**   | `id` (int): ID del egreso.                                                    |
| **Body**         | `form-data` con campo `File` (archivo).                                       |
| **Validaciones** | Extensiones: `.jpg`, `.png`, `.pdf`. Tamaño máx: 5MB.                         |
| **Respuesta**    | `ExpenseAttachmentDto` con metadata del archivo subido.                       |

### 2. Descargar Adjunto (Download Attachment)

| Propiedad       | Valor                                                                                                             |
| --------------- | ----------------------------------------------------------------------------------------------------------------- |
| **Método**      | `GET`                                                                                                             |
| **Ruta**        | `/api/attachments/{id}`                                                                                           |
| **Descripción** | Descarga el archivo adjunto asociado al ID proporcionado. Valida que el usuario pertenezca al Tenant del archivo. |
| **Parámetros**  | `id` (Guid): ID del adjunto.                                                                                      |
| **Respuesta**   | Archivo binario (`FileStreamResult`).                                                                             |

### 3. Listar Adjuntos (List Attachments)

| Propiedad       | Valor                                                                                                  |
| --------------- | ------------------------------------------------------------------------------------------------------ |
| **Método**      | `GET`                                                                                                  |
| **Ruta**        | `/api/attachments`                                                                                     |
| **Descripción** | Lista todos los adjuntos con filtros y paginación.                                                     |
| **Parámetros**  | `ExpenseId`, `UploadedFrom`, `UploadedTo`, `ContentType`, `PageNumber` (def: 1), `PageSize` (def: 20). |
| **Respuesta**   | `{ Items: [...], TotalCount, PageNumber, PageSize }`.                                                  |

---

## Servicios

**Ubicación:** `src/Consulcon.Infrastructure/Services/Contabilidad/ExpenseAttachmentService.cs`

- Implementa `IExpenseAttachmentService`.
- Manejo de almacenamiento físico (`LocalFileStorageStrategy`).
- Validación de archivos (tipo, tamaño, magic numbers).
- Creación de registro en BD y recuperación de stream para descarga.

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02-postman/postman_collection.json`  
**Carpeta:** `Otros > Egreso`

### Request 1: Upload Attachment

| Campo       | Valor                                                                  |
| ----------- | ---------------------------------------------------------------------- |
| **Nombre**  | Upload Attachment                                                      |
| **Método**  | `POST`                                                                 |
| **URL**     | `{{baseUrl}}/api/expenses/{{egresoId}}/attachments`                    |
| **Headers** | `Authorization: Bearer {{authToken}}`, `X-Tenant-Id: {{tenantId}}`     |
| **Body**    | `form-data` → `File` (tipo File, seleccionar archivo .pdf, .jpg, .png) |

### Request 2: Download Attachment

| Campo       | Valor                                                              |
| ----------- | ------------------------------------------------------------------ |
| **Nombre**  | Download Attachment                                                |
| **Método**  | `GET`                                                              |
| **URL**     | `{{baseUrl}}/api/attachments/{{attachmentId}}`                     |
| **Headers** | `Authorization: Bearer {{authToken}}`, `X-Tenant-Id: {{tenantId}}` |
| **Body**    | N/A                                                                |

### Request 3: List Attachments

| Campo       | Valor                                                                                                           |
| ----------- | --------------------------------------------------------------------------------------------------------------- |
| **Nombre**  | List Attachments                                                                                                |
| **Método**  | `GET`                                                                                                           |
| **URL**     | `{{baseUrl}}/api/attachments?ExpenseId=&UploadedFrom=&UploadedTo=&ContentType=&PageNumber=1&PageSize=20`        |
| **Headers** | `Authorization: Bearer {{authToken}}`, `X-Tenant-Id: {{tenantId}}`                                              |
| **Query**   | `ExpenseId` (int), `UploadedFrom` (date), `UploadedTo` (date), `ContentType` (string), `PageNumber`, `PageSize` |
