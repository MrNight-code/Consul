# 06. Dashboard Resumen Global

**Sprint:** 03
**Tipo:** Addition
**Fecha:** 28/01/2026

## Visión General

Generar endpoints de Dashboard que muestren contadores reales de:

- Total Unidades del Condominio
- Unidades en Mora
- Total Cobrado (Mes Actual)
- Porcentaje de Cobranza

El objetivo es permitir al Administrador tener una visión clara de la salud del condominio al entrar, basándose en registros reales de cobranzas y deudas.

## Nuevas Entidades

No se crearon nuevas entidades de dominio. Se reutilizaron las entidades existentes:

- `Propiedad` (Módulo Inmuebles)
- `DeudaCabecera` (Módulo Deudas)
- `TransaccionPago` (Módulo Cobranzas)

## Nuevos DTOs

Ubicación: `src/Consulcon.Application/DTOs/Dashboard/`

- `DashboardCountersDto`: Contiene los contadores para el dashboard.
  - `TotalUnidades`: Cantidad total de unidades activas.
  - `UnidadesEnMora`: Cantidad de unidades con deuda pendiente.
  - `TotalCobradoMesActual`: Suma de pagos registrados en el mes actual.
  - `PorcentajeCobranza`: Indicador de eficiencia de cobranza.
  - `CondominioNombre`: Nombre del condominio.
  - `UltimaActualizacion`: Fecha y hora del cálculo.

## Nuevo Controller

Ubicación: `src/Consulcon.API/Controllers/DashboardController.cs`

- Ruta Base: `api/Dashboard`

### Endpoints

#### Dashboard Operations

- `GET /api/Dashboard/{condominioId}`: Obtener contadores actuales. Retorna `DashboardCountersDto`.
- `POST /api/Dashboard/{condominioId}/refrescar`: Refrescar y recalcular contadores en tiempo real. Retorna `DashboardCountersDto` actualizado.

## Servicios

Ubicación: `src/Consulcon.Infrastructure/Services/Dashboard/DashboardService.cs`

- Implementa `IDashboardService`.
- **Funcionalidad**: Actúa como agregador de datos.
- **Interacción DB**:
  - Consulta `IPropiedadRepository` para contar unidades.
  - Consulta `IDeudaRepository` para unidades en mora.
  - Consulta `ITransaccionPagoRepository` para el total cobrado.

## Verificación y Calidad

### Resumen de Implementación

- **Estado**: ✅ COMPLETADO Y TESTADO
- **Criterios de Aceptación Cumplidos**:
  - ✅ Los números coinciden con la realidad de la BD.
  - ✅ Se implementa endpoint de "Refrescar".
  - ✅ Conexión correcta con datos de Cobranzas y Deudas.
  - ✅ Tests E2E implementados y pasando.

### Tests E2E Implementados `DashboardE2ETests.cs`

| Test                                                          | Objetivo                           | Estado |
| ------------------------------------------------------------- | ---------------------------------- | ------ |
| `GetContadores_WithValidCondominioId_ReturnsOk`               | Validar respuesta exitosa (200 OK) | ✅     |
| `GetContadores_WithInvalidCondominioId_ReturnsNotFound`       | Validar error 404                  | ✅     |
| `RefrescarContadores_WithValidCondominioId_ReturnsOk`         | Validar endpoint POST refrescar    | ✅     |
| `RefrescarContadores_WithInvalidCondominioId_ReturnsNotFound` | Validar validación POST            | ✅     |
| `UnidadesEnMoraShouldNotExceedTotalUnidades`                  | Regla de negocio: Mora <= Total    | ✅     |
| `PorcentajeCobranzaShouldBeValidRange`                        | Regla de negocio: 0 <= % <= 100    | ✅     |

### Métricas

- **Lines of Code (new)**: 527
- **Test Coverage**: 6 tests E2E
- **Compilación**: Clean (0 Errors, 0 Warnings)

## Detalles de Implementación Adicionales

### Decisiones de Diseño

1. **Endpoint Unificado**: Se optó por un solo DTO con todos los contadores para reducir la latencia (1 request vs múltiples).
2. **Cálculo en Tiempo Real**: Se decidió calcular al vuelo con opción de refresco explícito para garantizar datos frescos sin complejidad de caché distribuido por ahora.

### Arquitectura (Clean Architecture)

```
Domain
├─ Entities (Propiedad, DeudaCabecera, TransaccionPago)
 Application
├─ DTOs/Dashboard
├─ Interfaces/Dashboard
└─ Services/Dashboard
 API
└─ Controllers/DashboardController
```
