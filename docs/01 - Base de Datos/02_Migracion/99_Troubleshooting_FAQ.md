# FAQ y Troubleshooting de Migraciones

## Convenciones de Nombres de Bases de Datos

### ✅ Nombres Correctos

| Tipo de Base de Datos   | Formato                  | Ejemplo                 |
| :---------------------- | :----------------------- | :---------------------- |
| **Tenant (Producción)** | `db_condominio_{nombre}` | `db_condominio_bosques` |
| **Staging (Temporal)**  | `temp_import_{nombre}`   | `temp_import_bosques`   |

> [!IMPORTANT] > **NO uses nombres hardcoded** en los scripts de migración. Siempre usa los placeholders `{{STAGING_DB}}` y `{{TARGET_DB}}`.

### ❌ Errores Comunes

- ~~`db_condominio_bosques_colina`~~ → Nombre inconsistente, mezcla ID con nombre largo
- ~~`db_temp_bosques`~~ → Prefijo incorrecto (`db_` en vez de `temp_`)
- ~~`bdbosquescolina`~~ → Nombre legacy, no seguir este patrón

---

## Limpieza de Bases de Datos

### ¿Cuándo limpiar?

**DESPUÉS** de verificar que la migración fue exitosa:

```sql
-- 1. Verificar datos en la DB migrada
USE db_condominio_bosques;
SELECT COUNT(*) FROM propiedad;  -- Debe coincidir con el count de staging

-- 2. Si todo está OK, eliminar staging
DROP DATABASE IF EXISTS temp_import_bosques;
```

### ⚠️ Advertencia: Múltiples Migraciones

Si estás migrando **varios condominios en paralelo** (ej: Bosques y Foret al mismo tiempo):

- **NO elimines** bases de staging hasta que TODAS las migraciones hayan terminado
- Cada condominio tiene su propia `temp_import_{nombre}`
- La limpieza debe ser manual, **nunca automática**

---

## Conexión a CloudBeaver (DBeaver Web)

CloudBeaver corre **dentro de Docker** junto con MySQL. Usa esta configuración:

| Campo        | Valor                   | ❌ NO usar      |
| :----------- | :---------------------- | :-------------- |
| **Host**     | `db`                    | ~~`localhost`~~ |
| **Port**     | `3306`                  | ~~`3310`~~      |
| **Database** | `db_condominio_bosques` | ~~`db`~~        |
| **Username** | `root`                  |                 |
| **Password** | `root`                  |                 |

### ¿Por qué `db` y no `localhost`?

- CloudBeaver es un contenedor "hermano" de MySQL
- `localhost` para CloudBeaver = él mismo
- `db` = nombre del servicio MySQL en `docker-compose.yml`

### ¿Por qué `3306` y no `3310`?

- `3310` es el puerto **externo** (Windows → Docker)
- `3306` es el puerto **interno** (Docker → Docker)
- CloudBeaver debe usar el puerto interno

---

## Errores de Migración

### 1. `Field 'X' doesn't have a default value`

**Causa**: El script de migración no incluye una columna requerida.

**Solución**: Actualiza el script de transformación:

```sql
-- ❌ Incorrecto
INSERT INTO banco (id_banco, nombre_entidad)
SELECT pk_banco, descripcion FROM {{STAGING_DB}}.banco;

-- ✅ Correcto
INSERT INTO banco (id_banco, nombre_entidad, Tipo)
SELECT pk_banco, descripcion, 'Cuenta Corriente' FROM {{STAGING_DB}}.banco;
```

Siempre verifica el schema de destino con:

```sql
DESCRIBE tabla_destino;
```

### 2. `Unknown column 'id_usuario' in 'field list'`

**Causa**: El nombre de columna en el script no coincide con el schema.

**Ejemplo real**: La tabla `usuario` usa `pk_usuario` como PK, no `id_usuario`.

**Solución**: Revisa `ConsulconDbContext.cs` para confirmar el mapeo:

```csharp
entity.Property(e => e.IdUsuario).HasColumnName("pk_usuario");
```

### 3. `Unknown column 'saldo_deudor'`

**Causa**: Falta configuración de mapeo en EF Core.

**Solución**: Agrega el mapeo en `ConsulconDbContext.cs`:

```csharp
entity.Property(e => e.SaldoDeudor)
    .HasPrecision(12, 2)
    .HasDefaultValueSql("'0.00'")
    .HasColumnName("saldo_deudor");
```

Luego:

1. Rebuilda el proyecto
2. Rebuilda el Docker image: `docker-compose up -d --build api`
3. Reintenta la migración

### 4. `Connection refused` (CloudBeaver)

Ver sección "Conexión a CloudBeaver" arriba.

### 5. Datos faltantes después de migración

**Verificación paso a paso:**

```sql
-- 1. Verifica datos en staging
USE temp_import_bosques;
SELECT COUNT(*) FROM condominio;  -- Legacy
SELECT COUNT(*) FROM propiedad;   -- Legacy

-- 2. Verifica datos migrados
USE db_condominio_bosques;
SELECT COUNT(*) FROM condominio;  -- Debe ser 1
SELECT COUNT(*) FROM propiedad;   -- Debe coincidir con staging
```

Si los counts no coinciden:

- Revisa el output del script durante "Transformation"
- Busca errores SQL en la ejecución de `migrated_*.sql`

---

## Convención Multi-Tenancy

### ¿Por qué `condominio` tiene solo 1 fila?

En el **nuevo sistema**, cada condominio tiene **su propia base de datos**:

```
db_condominio_bosques  → 1 condominio (Bosques)
db_condominio_foret    → 1 condominio (Foret)
```

La tabla `condominio` en cada DB contiene la **configuración específica** de ese condominio (nombre, dirección, logo, admin).

### Legacy vs Nuevo Sistema

| Aspecto            | Legacy               | Nuevo (Multi-Tenant)     |
| :----------------- | :------------------- | :----------------------- |
| Arquitectura       | 1 DB → N condominios | N DBs → 1 condominio c/u |
| Tabla `condominio` | Múltiples filas      | **1 fila (Singleton)**   |
| Aislamiento        | Por código (WHERE)   | Por base de datos        |

---

## Checklist Pre-Migración

Antes de ejecutar `Import-LegacyDatabase.ps1`:

- [ ] Backup SQL existe y es accesible
- [ ] Script de transformación usa `{{STAGING_DB}}` y `{{TARGET_DB}}`
- [ ] Verificaste schema de destino (`DESCRIBE tabla`)
- [ ] `ConsulconDbContext.cs` tiene los mappings correctos
- [ ] Docker containers están corriendo (`docker ps`)

## Checklist Post-Migración

- [ ] Counts de staging coinciden con counts de destino
- [ ] La tabla `condominio` tiene **exactamente 1 fila**
- [ ] El campo `saldo_deudor` en `propiedad` tiene valores (default: 0.00)
- [ ] Puedes conectarte a CloudBeaver con `db:3306`
- [ ] Limpiaste bases de datos obsoletas (manual)
