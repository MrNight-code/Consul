# 07. Valores Hardcodeados Detectados - Sprint 04

**Sprint:** 04  
**Tipo:** Change (Pendiente)  
**Fecha:** 11/03/2026  
**Módulo:** Facturación / Contabilidad

## Descripción

Durante la revisión post-merge de las ramas Benjamin y Paul, se identificaron los siguientes valores hardcodeados que podrían causar problemas en producción o dificultar la mantenibilidad del sistema.

## Valores Detectados

### 1. Ruta de Carpeta `"GeneratedReceipts"` (PRIORIDAD ALTA)

| Propiedad      | Valor                                                                      |
| -------------- | -------------------------------------------------------------------------- |
| **Archivos**   | `ReceiptGenerationService.cs` (línea 23), `PagoController.cs` (línea 50)  |
| **Valor**      | `"GeneratedReceipts"`                                                      |
| **Riesgo**     | La ruta es relativa al directorio de trabajo, lo cual es frágil en Docker  |
| **Sugerencia** | Mover a `appsettings.json` como `ReceiptSettings:OutputFolder`             |

```csharp
// Actual (hardcodeado)
private const string OutputFolder = "GeneratedReceipts";

// Sugerido
var outputFolder = configuration["ReceiptSettings:OutputFolder"];
```

---

### 2. Zona Horaria Bolivia `TimeSpan.FromHours(-4)` (PRIORIDAD MEDIA)

| Propiedad      | Valor                                                                     |
| -------------- | ------------------------------------------------------------------------- |
| **Archivo**    | `ReceiptGenerationService.cs` (líneas 59 y 292)                          |
| **Valor**      | `TimeSpan.FromHours(-4)`                                                  |
| **Riesgo**     | Si el sistema se usa en otro país, la hora en los recibos será incorrecta |
| **Sugerencia** | Usar `TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time")`    |

```csharp
// Actual (hardcodeado)
var boliviaOffset = TimeSpan.FromHours(-4);

// Sugerido
var boliviaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
var serverTimeBolivia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, boliviaTimeZone);
```

---

### 3. Fallback `"Cliente General"` y `"S/M"` (PRIORIDAD BAJA)

| Propiedad      | Valor                                                   |
| -------------- | ------------------------------------------------------- |
| **Archivo**    | `ReceiptGenerationService.cs` (líneas 233-234)          |
| **Valor**      | `"Cliente General"` y `"S/M"`                           |
| **Riesgo**     | Texto visible en reportes, difícil de internacionalizar |
| **Sugerencia** | Mover a constantes de recursos o configuración          |

---

### 4. Fallback `"Unknown User"` en `ExpensesController` (PRIORIDAD BAJA)

| Propiedad      | Valor                                            |
| -------------- | ------------------------------------------------ |
| **Archivo**    | `ExpensesController.cs` (línea 29)               |
| **Valor**      | `"Unknown User"`                                 |
| **Riesgo**     | Si no se encuentra claim, se permite la operación con nombre genérico |
| **Sugerencia** | Considerar retornar `Unauthorized` en lugar de un fallback           |

### 5. Valores por defecto y nombres de hojas en Excel (PRIORIDAD BAJA)

| Propiedad      | Valor                                            |
| -------------- | ------------------------------------------------ |
| **Archivo**    | `ExcelService.cs` (línea 11)                     |
| **Valor**      | `"Reporte"` y formatos `dd/MM/yyyy HH:mm`, `#,##0.00` |
| **Riesgo**     | Si el negocio es internacional, los formatos fijos pueden aplicar mal, y el nombre de hoja no está regionalizado. |
| **Sugerencia** | Considerar recibir la cultura (CultureInfo) o configuraciones globales del Tenant para el formatting. |

## Resumen de Prioridades

| #  | Valor Hardcodeado              | Prioridad | Estado    |
| -- | ------------------------------ | --------- | --------- |
| 1  | `"GeneratedReceipts"`          | 🔴 Alta   | ✅ Corregido |
| 2  | `FromHours(-4)`                | 🟡 Media  | ✅ Corregido |
| 3  | `"Cliente General"`            | 🟢 Baja   | ✅ Corregido |
| 4  | `"Unknown User"`               | 🟢 Baja   | ✅ Corregido |
| 5  | Nombres de Hoja/Formatos Excel | 🟢 Baja   | ✅ Corregido |
