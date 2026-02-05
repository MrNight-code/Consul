# Documentación de Cambios: Dashboard y Recibos (Sprint 03)

## Resumen

Este documento detalla la implementación de dos nuevas características principales: el **Servicio de Métricas del Dashboard** y el **Sistema de Generación de Recibos**. Ambos componentes han sido diseñados siguiendo estrictos criterios de arquitectura limpia, inmutabilidad y seguridad.

---

## 1. Dashboard Metrics Service

### Descripción

Servicio encargado de calcular y agregar métricas financieras clave para la pantalla principal del administrador. Reemplaza los datos simulados por cálculos en tiempo real sobre la base de datos transaccional.

### Endpoint

`GET /api/Dashboard/{condominioId}`

### Métricas Calculadas

1.  **Total Unidades**: Conteo de propiedades activas.
2.  **Unidades en Mora**: Propiedades con `SaldoDeudor > 0`.
3.  **Total Mora Histórica**: Suma total de deuda de todas las unidades.
4.  **Eficiencia de Cobranza**: `(Total Cobrado Mes / Total Deuda Generada Mes) * 100`.
5.  **Cash Flow**: `Ingresos (Transacciones de Pago)` - `Egresos (Gastos Registrados)`.
6.  **Total Egresos**: Suma de gastos del mes actual.

### Cambios Técnicos

- **Nueva Interfaz**: `IDashboardMetricsService`.
- **Nueva Implementación**: `DashboardMetricsService`.
- **DTO Actualizado**: `DashboardCountersDto` extendido con nuevos campos (`TotalMoraHistorica`, `CashFlowMesActual`, etc.).
- **Controller Refactorizado**: `DashboardController` ahora utiliza Primary Constructor y delega en el nuevo servicio.

---

## 2. Receipt Generation System (Recibos)

### Descripción

Sistema para la generación de recibos de pago en formato PDF con firma temporal inmutable del servidor.

### Endpoint

`POST /api/Receipt/generate/{transaccionId}`

### Características Clave (RN-001)

- **Timestamp Inmutable**: La fecha y hora de generación (`FechaRecibo`) se obtiene estrictamente del servidor (`DateTime.UtcNow`).
- **Marca de Agua**: El PDF incluye una marca de agua "ORIGINAL [YYYY-MM-DD]" rotada a 45 grados.
- **Persistencia**: No se crea una tabla nueva. Se reutiliza la tabla `TransaccionPago`, actualizando los campos `ReciboUrl` y `FechaRecibo`.

### Componentes Nuevo

- **Librería**: `QuestPDF` añadida a `Consulcon.Infrastructure`.
- **Refactorización**: `TransaccionPago` ahora almacena la metadata del recibo.
- **Servicio**: `ReceiptGenerationService` actualizado para trabajar con transacciones.
- **Controller**: `ReceiptController` expone `GenerateReceipt(int transaccionId)`.

---

## 3. Pruebas y Postman

### Postman

La colección ha sido actualizada con una carpeta `Dashboard Metrics` y `Facturacion` que incluyen los endpoints mencionados.

### Tests de Integración

Se actualizaron los tests de integración (`Consulcon.IntegrationTests`) para verificar que el servicio de recibos persiste correctamente los datos usando una base de datos en memoria y valida la integridad del archivo PDF generado.

### Correcciones Adicionales

- Se corrigieron advertencias del IDE (Primary Constructors, Dispose Pattern).
- Se resolvió ambigüedad con la clase `Result` eliminando la dependencia innecesaria de `FluentResults`.
