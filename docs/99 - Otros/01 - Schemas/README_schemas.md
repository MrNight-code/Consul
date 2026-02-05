# Schemas para dbdiagram.io - Comparación Sistema Legacy vs Nuevo

Este directorio contiene los archivos SQL para visualizar en [dbdiagram.io](https://dbdiagram.io) las diferencias entre el sistema legacy y el nuevo.

## 📋 Archivos

### 1. `schema_legacy_bosques.sql`

**Base de datos origen:** `syscons1_bdbosquescolina`

Schema COMPLETO del sistema legacy con **53 tablas** exactas extraídas del archivo original.

#### ✅ Características:

- Extracción literal y completa de todas las tablas
- 53 tablas (incluye tablas temporales y de reportes)
- Todas las columnas originales preservadas
- Referencias (foreign keys) completas
- Nomenclatura original con prefijos `pk_` y `fk_`

#### 📊 Tablas Principales (19):

- **Configuración**: `autorizacion`, `banco`, `formpago`, `serviciopago`, `tipousuario`, `moneda`, `opcioncobro`, `permiso`
- **Personas**: `persona`, `telefono`, `correo`, `proveedor`
- **Usuarios**: `usuario`, `tipo_permiso`
- **Inmobiliario**: `condominio`, `manzano`, `propiedad`
- **Contratos**: `contrato`, `persona_contrato`, `servicio_contrato`
- **Finanzas**: `deuda`, `cuota`, `egreso`
- **Contabilidad**: `asiento`, `detalleasiento`, `cuenta`
- **Eventos**: `evento`, `blog`
- **Empresa**: `empresa`

#### 🔧 Tablas de Configuración (7):

- `avisoconbranza`, `confaviso`, `confejecucion`, `confejecuciontitulo`, `detalleconfejecucion`, `configuracionevento`, `cuentasistema`, `serviciocuenta`

#### 📝 Tablas de Recibos (3):

- `reciboegreso`, `recibogeneral`, `recibopersonal`

#### 📋 Tablas de Reportes y Temporal (14):

- `tabla_servicio`, `TbReporteFinal`, `tbreportefinal`
- `TempContratoServicio`, `tempcontratoservicio`
- `TempTablaMadre`, `temptablamadre`
- `TempTotalTable`, `temptotaltable`
- `temResultado`, `temresultado`
- `ttcuota`, `ttdeuda`

---

### 2. `schema_migration_template.sql`

**Base de datos destino:** Nuevo sistema Backend-Consulcon

Schema normalizado del sistema nuevo con **30 tablas** optimizadas.

#### ✅ Características:

- Estructura normalizada y moderna
- Relaciones claras con foreign keys
- Uso de `boolean` para flags
- Separación cabecera/detalle
- Sistema de permisos implementado

#### 📊 Tablas (30):

- **Configuración**: `rol`, `banco`, `forma_pago`, `catalogo_servicio`, `autorizacion_gasto`, `plan_cuentas`
- **Personas**: `persona`, `medio_contacto`, `proveedor`
- **Usuarios y Permisos**: `usuario`, `permiso`, `rol_permiso`
- **Inmobiliario**: `condominio`, `config_aviso_cobranza`, `manzano`, `propiedad`
- **Contratos**: `contrato`, `contrato_participante`, `contrato_servicio_suscrito`, `lectura_servicio`
- **Finanzas**: `deuda_cabecera`, `deuda_detalle`, `transaccion_pago`, `egreso`
- **Contabilidad**: `asiento_contable`, `asiento_detalle`
- **Extras**: `recurso_comun`, `reserva`, `comunicado_blog`

---

## 🔍 Comparación: 53 vs 30 Tablas

### ❌ Tablas Eliminadas del Legacy (23)

**Tablas Temporales** (eliminadas - eran solo para reportes):

- `tabla_servicio`, `TbReporteFinal`, `tbreportefinal`
- `TempContratoServicio`, `tempcontratoservicio`
- `TempTablaMadre`, `temptablamadre`
- `TempTotalTable`, `temptotaltable`
- `temResultado`, `temresultado`
- `ttcuota`, `ttdeuda`

**Tablas de Recibos** (consolidadas en sistema de comprobantes):

- `reciboegreso`, `recibogeneral`, `recibopersonal`

**Tablas de Configuración Antigua** (reemplazadas por config moderna):

- `avisoconbranza` → `config_aviso_cobranza`
- `confaviso`, `confejecucion`, `confejecuciontitulo`, `detalleconfejecucion`
- `configuracionevento` → `recurso_comun`

**Otras**:

- `empresa` (info ahora en configuración general)
- `moneda` (simplificado - solo BOB)
- `opcioncobro` (integrado en `forma_pago`)
- `serviciocuenta` (integrado en plan de cuentas)

### ✅ Tablas Nuevas en el Sistema Moderno (7)

1. **`medio_contacto`** - Unifica `telefono`, `correo` y campos en `persona`
2. **`deuda_cabecera`** + **`deuda_detalle`** - Separa cabecera/detalle (antes `deuda` era una sola tabla)
3. **`transaccion_pago`** - Renombra y mejora `cuota`
4. **`asiento_contable`** + **`asiento_detalle`** - Separa cabecera/detalle (antes `asiento` + `detalleasiento`)
5. **`config_aviso_cobranza`** - Configuración moderna de avisos
6. **`rol_permiso`** - Reemplaza `tipo_permiso` con mejor diseño
7. **`lectura_servicio`** - Nueva funcionalidad para lecturas de medidores
8. **`recurso_comun`** + **`reserva`** - Reemplaza `evento` con mejor diseño
9. **`comunicado_blog`** - Renombra `blog`
10. **`contrato_participante`** - Renombra `persona_contrato`
11. **`contrato_servicio_suscrito`** - Renombra `servicio_contrato`

---

## 🎯 Cómo usar

1. Ve a https://dbdiagram.io
2. Copia el contenido de cualquiera de los archivos `.sql`
3. Pégalo en el editor
4. ¡El diagrama se genera automáticamente!

---

## 📊 Principales Diferencias

| Aspecto               | Legacy (53 tablas)                        | Nuevo Sistema (30 tablas)             |
| --------------------- | ----------------------------------------- | ------------------------------------- |
| **Total de Tablas**   | 53                                        | 30                                    |
| **Tablas de Negocio** | 19                                        | 26                                    |
| **Tablas Temporales** | 14                                        | 0 (generadas en tiempo real)          |
| **Contactos**         | `telefono`, `correo`, campos en `persona` | `medio_contacto` (normalizada)        |
| **Deudas**            | `deuda` simple                            | `deuda_cabecera` + `deuda_detalle`    |
| **Pagos**             | `cuota`                                   | `transaccion_pago`                    |
| **Permisos**          | `tipo_permiso`                            | `rol_permiso`, `permiso`              |
| **Eventos**           | `evento`, `configuracionevento`           | `recurso_comun` + `reserva`           |
| **Blog**              | `blog`                                    | `comunicado_blog`                     |
| **Asientos**          | `asiento`, `detalleasiento`               | `asiento_contable`, `asiento_detalle` |
| **Booleanos**         | `varchar` ('SI'/'NO')                     | `boolean` nativo                      |
| **Recibos**           | 3 tablas separadas                        | Sistema unificado                     |
| **Configuración**     | 7 tablas                                  | 1 tabla (`config_aviso_cobranza`)     |

---

## 📈 Mejoras en el Nuevo Sistema

### ✅ Normalización

- **Contactos unificados**: Una tabla `medio_contacto` vs 2 tablas + campos en persona
- **Estructura cabecera/detalle**: Separa datos maestros de detalles

### ✅ Eliminación de Redundancia

- **Sin tablas temporales**: 14 tablas de reporte eliminadas
- **Configuración simplificada**: 7 tablas → 1 tabla

### ✅ Nuevas Funcionalidades

- **Lecturas de servicios**: Registro de consumos de agua/luz
- **Sistema de permisos granular**: Mejora sobre el legacy
- **Gestión de recursos**: Mejor control de churrasqueras/s alones

### ✅ Mejores Prácticas

- **Tipos de datos correctos**: `boolean` vs `varchar`
- **Nombres descriptivos**: `comunicado_blog` vs `blog`
- **Relaciones claras**: Foreign keys bien definidas

---

## 🔗 Referencias

- **Legacy → Nuevo**: Ver `migration_template.sql` para el mapeo de migración
- **DBDiagram.io**: https://dbdiagram.io/d (diagrama interactivo)
- **Documentación**: Ver `/docs` para más detalles del sistema
