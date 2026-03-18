# 05. Validación de Unicidad de Nro de Recibo (Integridad) - Cambios

**Sprint:** 04
**Tipo:** Change
**Fecha:** 04/03/2026
**Módulo:** Facturación

## Descripción

Asegurar que no existan dos recibos para el mismo pago, previniendo inconsistencias legales y contables. Se agregó una validación temprana antes de la generación del recibo PDF para evitar duplicados.

## Cambios en Lógica de Negocio

- **Servicio Afectado**: `ReceiptGenerationService`
- **Nuevos Cambios**: Se insertó un bloque de validación temprana dentro de `GenerateReceiptAsync`. Antes de invocar a QuestPDF para generar el PDF, el servicio ahora verifica:
  1. Si el pago existe (`KeyNotFoundException` si no se encuentra).
  2. Si ya tiene un recibo generado (`InvalidOperationException` si la propiedad `ReciboUrl` ya contiene un valor).
- **Flujo de Datos**: La validación respeta el ámbito del condominio de forma intrínseca. Dado que el sistema es Multi-Tenant con bases de datos separadas por condominio, el `DbContext` inyectado automáticamente limita las consultas al condominio autenticado.

## Impacto

- Intento de generar un recibo sobre un pago que ya cuenta con uno lanzará una excepción, la cual será interceptada por el controlador devolviendo `400 Bad Request` u otro código dictado por el middleware.
- Previene almacenamiento duplicado o pérdida de seguimiento del archivo original.

## Deuda Técnica Detectada

Si un archivo PDF es eliminado manualmente del servidor (carpeta `GeneratedReceipts`), la base de datos sigue registrando que el recibo existe. Esto deja al pago en un estado "zombi" donde no se puede generar el recibo nuevamente (la BBDD dice que existe) ni descargarlo (el archivo comprobador no existe).

**Sugerencia:** Agregar validación con `System.IO.File.Exists(Path.GetFullPath(pago.ReciboUrl))` antes de lanzar la excepción para detectar este escenario de archivo huérfano.
