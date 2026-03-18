# 📊 Dashboard - Contadores Agregados del Condominio

## 📋 Descripción

El módulo Dashboard proporciona una vista unificada de los principales indicadores de gestión de un condominio:

- **Total Unidades**: Cantidad de propiedades activas registradas
- **Unidades en Mora**: Propiedades con deuda pendiente
- **Total Cobrado (Mes Actual)**: Suma de pagos registrados desde el 1 hasta hoy
- **Porcentaje de Cobranza**: % de cobranza = (Total Cobrado / Total Deuda) × 100

---

## 🔌 Endpoints

### 1. Obtener Contadores
```
GET /api/Dashboard/{condominioId}
```

**Parámetros:**
- `condominioId` (path): ID del condominio

**Headers:**
```
Authorization: Bearer <token_jwt>
X-Tenant-Id: <nombre_condominio>
```

**Respuesta Exitosa (200):**
```json
{
  "totalUnidades": 45,
  "unidadesEnMora": 8,
  "totalCobradoMesActual": 15000.50,
  "porcentajeCobranza": 78.50,
  "condominioNombre": "Condominio Vista Verde",
  "ultimaActualizacion": "2026-01-28T14:30:45.123Z"
}
```

---

### 2. Refrescar Contadores
```
POST /api/Dashboard/{condominioId}/refrescar
```

Refresca/recalcula los contadores con datos actuales (sin caché).

**Parámetros:**
- `condominioId` (path): ID del condominio

**Headers:**
```
Authorization: Bearer <token_jwt>
X-Tenant-Id: <nombre_condominio>
```

**Body:** (vacío)

**Respuesta Exitosa (200):**
```json
{
  "totalUnidades": 45,
  "unidadesEnMora": 8,
  "totalCobradoMesActual": 15000.50,
  "porcentajeCobranza": 78.50,
  "condominioNombre": "Condominio Vista Verde",
  "ultimaActualizacion": "2026-01-28T14:32:10.456Z"
}
```

---

## 🧪 Tests E2E

Se han creado tests automatizados en `DashboardE2ETests.cs`:

### Casos de Test

1. **GetContadores_WithValidCondominioId_ReturnsOk**
   - Valida que retorna 200 OK
   - Verifica estructura del DTO
   - Comprueba que UnidadesEnMora ≤ TotalUnidades

2. **GetContadores_WithInvalidCondominioId_ReturnsNotFound**
   - Verifica que retorna 404 para condominio inexistente

3. **RefrescarContadores_WithValidCondominioId_ReturnsOk**
   - Valida que endpoint POST funciona correctamente

4. **RefrescarContadores_WithInvalidCondominioId_ReturnsNotFound**
   - Verifica validación de condominio en refrescar

5. **PorcentajeCobranzaShouldBeValidRange**
   - Valida que % esté entre 0 y 100

### Ejecutar Tests

```bash
cd tests/Consulcon.IntegrationTests
dotnet test DashboardE2ETests.cs -v
```

---

## 📊 Lógica de Cálculo

### Total Unidades
```sql
COUNT(Propiedad) WHERE IdCondominio = {id} AND Activo = true
```

### Unidades en Mora
```sql
COUNT(Propiedad) WHERE IdCondominio = {id} AND SaldoDeudor > 0
```

### Total Cobrado Mes Actual
```sql
SUM(TransaccionPago.MontoAbonado) 
WHERE MONTH(FechaPago) = MONTH(GETDATE())
  AND YEAR(FechaPago) = YEAR(GETDATE())
  AND IdCondominio = {id}
```

### Porcentaje de Cobranza
```
% = (TotalCobradoMesActual / TotalDeudaMesActual) × 100
```

---

## 🏗️ Arquitectura

```
┌─────────────────────────────────────┐
│      DashboardController            │
│  GET /api/Dashboard/{condominioId}  │
│  POST /api/Dashboard/{id}/refrescar │
└──────────────────┬──────────────────┘
                   │
┌──────────────────▼──────────────────┐
│         DashboardService            │
│  • ObtenerContadoresAsync()         │
│  • RefrescarContadoresAsync()       │
│  • CalcularContadoresAsync()        │
└──────────────┬───────────┬──────────┘
               │           │
      ┌────────▼──┐  ┌─────▼────────┐
      │Propiedad  │  │DeudaCabecera │
      │Repository │  │Repository    │
      └────────────┘  └──────────────┘
                 │
          ┌──────▼─────────┐
          │TransaccionPago │
          │Repository      │
          └────────────────┘
```

---

## 🔍 Validaciones

- ✅ Condominio debe existir (retorna 404 si no)
- ✅ UnidadesEnMora ≤ TotalUnidades
- ✅ PorcentajeCobranza entre 0-100
- ✅ TotalCobradoMesActual ≥ 0
- ✅ UltimaActualizacion siempre al momento actual

---

## 📝 Ejemplo Postman

```json
{
  "name": "Dashboard - Get Contadores",
  "request": {
    "method": "GET",
    "header": [
      {
        "key": "Authorization",
        "value": "Bearer {{authToken}}",
        "type": "text"
      },
      {
        "key": "X-Tenant-Id",
        "value": "{{tenantId}}",
        "type": "text"
      }
    ],
    "url": {
      "raw": "{{baseUrl}}/api/Dashboard/1",
      "host": ["{{baseUrl}}"],
      "path": ["api", "Dashboard", "1"]
    }
  }
}
```

---

## 🚀 Próximas Mejoras (Opcional)

- [ ] Implementar caché de contadores (con invalidación al registrar pago/deuda)
- [ ] Agregar endpoint para histórico de contadores por período
- [ ] Agregar gráficas de tendencia de cobranza
- [ ] Notificaciones de cambios en unidades en mora
- [ ] Exportar reporte a PDF

---

## ✅ Criterios de Aceptación Cumplidos

- ✅ Endpoint GET devuelve contadores reales desde BD
- ✅ Después de registrar cobranza, TotalCobradoMesActual aumenta
- ✅ UnidadesEnMora = propiedades con saldo_deudor > 0
- ✅ Endpoint POST para refrescar actualiza datos
- ✅ UltimaActualizacion siempre se actualiza
- ✅ Tests E2E validan todos los escenarios
