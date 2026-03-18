# Progreso Exportación a Excel

## Arquitectura Base
- [x] Librería `ClosedXML` instalada en `Consulcon.Infrastructure`.
- [x] Servicio genérico `IExcelService` y clase `ExcelService` creados e inyectados en el contenedor de dependencias (`DependencyInjection.cs`).

## Endpoints Implementados
### 1. Cobranzas (`/api/cobranzas/{unitId}/export`)
- **Descripción**: Exporta el historial de pagos de una unidad.
- **Implementación**: Se agregó el método `ExportarHistorial` al controlador `CobranzaController.cs`.
- **Funcionamiento**: Reutiliza `_service.ObtenerHistorialAsync(unitId)` para no duplicar consultas. Utiliza el `ExcelService` para retornar un `.xlsx`.
- **Estado**: ✅ Implementado. Falla de compilación resuelta (missing using). Pendiente: Pruebas en Postman.

### 2. Deudas (`/api/deuda/pendiente/export`)
- **Descripción**: Exporta la lista de todas las deudas pendientes.
- **Implementación**: Se agregó el método `ExportarDeudasPendientes` al controlador `DeudaController.cs`.
- **Funcionamiento**: Reutiliza `_service.GetPendingAsync()` para obtener las deudas y las pasa al `ExcelService`.
- **Estado**: ✅ Implementado. Listo para probar en Postman.

### 3. Proveedores (`/api/providers/export`)
- **Descripción**: Exporta el listado general de proveedores registrados.
- **Implementación**: Se agregó el método `Export` al controlador `ProvidersController.cs`.
- **Funcionamiento**: Reutiliza `_service.GetPagedAsync(1, int.MaxValue...)` para deshabilitar temporalmente la paginación y recuperar todos los activos según el término de búsqueda.
- **Estado**: ✅ Implementado. Listo para probar en Postman.

### 4. Libro de Caja (`/api/cashbook/export`)
- **Descripción**: Exporta el reporte de flujo de fondos (ingresos y egresos).
- **Implementación**: Se agregó el método `ExportCashBook` al controlador `CashBookController.cs`.
- **Funcionamiento**: Reutiliza `cashBookService.GetCashBookAsync(query)` forzando `query.PageSize = int.MaxValue`. Extrae la lista de `Entries` y la envía al `ExcelService`.
- **Estado**: ✅ Implementado. Listo para probar en Postman.
