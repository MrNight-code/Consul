# 07. Generación de Recibos de Pago

**Sprint:** 03
**Tipo:** Addition
**Fecha:** 25/08/2024

## Visión General

El sistema permite generar comprobantes de pago inmutables con fecha y hora del servidor (UTC). El PDF se almacena localmente y la ruta se guarda en la base de datos para futuras referencias.
Esta funcionalidad asegura la integridad de los comprobantes emitidos y facilita su recuperación.

## Nuevas Entidades

No se han creado nuevas entidades, pero se han utilizado propiedades existentes de `TransaccionPago` de manera específica para esta funcionalidad.

### `TransaccionPago` (Existente)

Ubicación: `src/Consulcon.Domain/Entities/Facturacion/TransaccionPago.cs`

- **Propiedades Utilizadas**:
  - `ReciboUrl`: Almacena la ruta absoluta del archivo PDF generado.
  - `FechaRecibo`: Almacena la fecha y hora (UTC) exacta de la generación del recibo.

## Nuevos DTOs

No se han creado DTOs específicos para la solicitud, ya que se utiliza el ID de la transacción directamente. La respuesta es un objeto anónimo JSON.

## Nuevo Controller

Ubicación: `src/Consulcon.API/Controllers/Facturacion/CobranzaController.cs` (Existente, endpoints agregados)

- Ruta Base: `api/cobranzas`

### Endpoints

#### Listado de Recibos

- `GET /api/cobranzas/recibos`: Lista los recibos generados con filtros opcionales.
  - **Parámetros (Query)**:
    - `FechaDesde` (DateTime, opcional): Inicio del rango de fecha.
    - `FechaHasta` (DateTime, opcional): Fin del rango de fecha.
    - `PersonaId` (int, opcional): Filtrar por pagador.
  - **Retorno**: Lista de objetos con ID, URL, Fecha, Monto y Nombre.

#### Generación de Recibos

- `POST /api/cobranzas/{id}/generar-recibo`: Genera un recibo PDF para una transacción de pago existente.
  - **Parámetros**: `id` (int) - ID de la `TransaccionPago`.
  - **Retorno**:
    ```json
    {
      "id": 1,
      "rutaPdf": "C:\\...",
      "fechaGeneracion": "2024-08-25T14:30:00Z",
      "mensaje": "..."
    }
    ```

#### Descarga de Recibos

- `GET /api/cobranzas/recibos/{filename}`: Descarga el archivo PDF generado.
  - **Parámetros**: `filename` (string) - Nombre del archivo retornado por el endpoint de generación.
  - **Retorno**: Archivo PDF (`application/pdf`).

## Servicios

Ubicación: `src/Consulcon.Infrastructure/Services/Facturacion/ReceiptGenerationService.cs`

- Implementa `IReceiptGenerationService`.

### Características Principales

1.  **Generación de PDF**: Utiliza la librería `QuestPDF` para generar recibos con diseño profesional.
2.  **Almacenamiento Local**: Los recibos se guardan en la carpeta `GeneratedReceipts` del servidor.
3.  **Persistencia**: El Docker Volume asegura que los archivos no se pierdan al reiniciar el contenedor.
4.  **Inmutabilidad**: El recibo generado NO se sobrescribe y actúa como una "foto" del momento del pago.
5.  **Formato Localizado**:
    - Moneda: Bolivianos (`bs`).
    - Hora: Hora de Bolivia (UTC-4).
6.  **Información Detallada**:
    - Tabla de "Conceptos de la Deuda" (desglose de ítems).
    - Banco, Forma de Pago, Nro. Operación y Observaciones.
    - Ocultamiento automático de campos vacíos.

- **Interacción DB**:
  - Utiliza `ConsulconDbContext` directemante para `Include` profundos y actualización de la entidad.

## Documentación y Pruebas

### Postman

- **Archivo de Colección**: `docs/99 - Otros/02-postman/postman_collection.json`
- **Ubicación del Endpoint**:
  - Carpeta: `Cobranzas`
  - Request: `Generar Recibo`

### Swagger / OpenAPI

- El endpoint se encuentra disponible en la documentación interactiva de Swagger bajo la sección **Pago**.
- Ruta relativa en UI: `/swagger`
