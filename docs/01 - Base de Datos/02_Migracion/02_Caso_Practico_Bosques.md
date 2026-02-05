# Caso Práctico: Migración de "Condominio Bosques"

Este documento explica paso a paso cómo migrar la base de datos del **Condominio Bosques** (Legacy) al nuevo sistema multi-tenant, tanto para entornos locales (Docker) como para la nube (Railway).

> [!NOTE]
> Para troubleshooting detallado, consulta [99_Troubleshooting_FAQ.md](99_Troubleshooting_FAQ.md)

## 📋 Pre-requisitos

1.  **Backup SQL**: El archivo se encuentra en el repositorio.
    - Ruta: `scripts/database/data/Bosques/syscons1_bdbosquescolina.sql`
2.  **Script de Transformación**: Adaptación de datos al nuevo esquema.
    - Ruta: `scripts/database/migrations/Bosques/migrated_bosques_colina.sql`
3.  **Credenciales Railway** (para nube): Host, Puerto, Usuario y Password.

---

## 🏠 Migración Local (Docker)

### Paso 1: Levantar Infraestructura

```powershell
docker-compose up -d
```

### Paso 2: Ejecutar Migración

```powershell
.\scripts\utils\Import-LegacyDatabase.ps1 `
    -SourceSqlDump "scripts\database\data\Bosques\syscons1_bdbosquescolina.sql" `
    -StagingDbName "db_temp_bosques" `
    -MigrationScript "scripts\database\migrations\Bosques\migrated_bosques_colina.sql" `
    -TargetDbName "db_condominio_bosques_colina"
```

> **Nota:** Los parámetros `-DbHost`, `-DbPort`, `-DbUser`, `-DbPassword` y `-UseDockerExec` son opcionales para local. El script usa los valores por defecto del contenedor Docker.

### Paso 3: Verificar

Abre CloudBeaver en [http://localhost:8978](http://localhost:8978) y revisa la BD `db_condominio_bosques_colina`.

---

## ☁️ Migración a Railway (Nube)

### Paso 1: Obtener Credenciales de Railway

1. Abre tu proyecto en [Railway.app](https://railway.app/)
2. Ve al servicio **MySQL**
3. En la pestaña **Connect**, copia:
   - **MYSQLHOST** (ej: `junction.proxy.rlwy.net`)
   - **MYSQLPORT** (ej: `54321`)
   - **MYSQLUSER** (usualmente `root`)
   - **MYSQLPASSWORD** (la contraseña generada)

### Paso 2: Ejecutar Migración Remota

```powershell
.\scripts\utils\Import-LegacyDatabase.ps1 `
    -SourceSqlDump "scripts\database\data\Bosques\syscons1_bdbosquescolina.sql" `
    -StagingDbName "db_temp_bosques" `
    -MigrationScript "scripts\database\migrations\Bosques\migrated_bosques_colina.sql" `
    -TargetDbName "db_condominio_bosques_colina" `
    -DbHost "junction.proxy.rlwy.net" `
    -DbPort "54321" `
    -DbUser "root" `
    -DbPassword "xX_TuPasswordDeRailway_Xx" `
    -UseDockerExec $false
```

> **Importante:**
>
> - Reemplaza los valores de `-DbHost`, `-DbPort` y `-DbPassword` con tus credenciales reales.
> - El parámetro `-UseDockerExec $false` es **crítico** para conexión remota.

### ¿Qué hace el script?

1.  Conecta a Railway usando las credenciales proporcionadas.
2.  Crea la base de datos `db_condominio_bosques_colina` usando la API en modo migración.
3.  Crea una base temporal `db_temp_bosques`.
4.  Importa el backup legacy a la base temporal.
5.  Ejecuta el script de transformación para adaptar los datos.
6.  Mueve los datos transformados a la BD definitiva.
7.  Limpia la base temporal.

### Paso 3: Verificación

1.  Abre Postman.
2.  Haz Login como administrador.
3.  Haz una petición a `GET /api/Propiedad` enviando el header `X-Tenant-Id: bosques_colina`.
4.  Deberías ver las propiedades que venían del sistema antiguo.

---

## 📂 Archivos Clave

Es fundamental entender qué hace cada archivo en este proceso:

1.  **Backup Legacy** (`syscons1_bdbosquescolina.sql`):

    - Es tu **Fuente de Datos** original (exportado de PHPMyAdmin).
    - Contiene la estructura antigua y los datos crudos.

2.  **Script de Transformación** (`migrated_bosques_colina.sql`):

    - Es el **Mapa** que dice cómo mover datos del viejo al nuevo.
    - Generado a partir de `generate_migration.py`.
    - Contiene instrucciones `INSERT INTO nueva_tabla SELECT FROM {{STAGING_DB}}.vieja_tabla`.

3.  **Script de Importación** (`Import-LegacyDatabase.ps1`):
    - Es el **Ejecutor** unificado para local y nube.
    - Sube el Backup → Crea una DB Temporal → Ejecuta la Transformación → Limpia.

---

## ❓ Solución de Problemas

### Error: "Unknown database 'db_condominio_bosques_colina'"

- **Causa:** La base de datos no fue creada automáticamente.
- **Solución:** Verifica que la API esté corriendo y tenga permisos `CREATE DATABASE` en el servidor MySQL.

### Error de conexión a Railway

- **Causa:** Credenciales incorrectas o firewall.
- **Solución:**
  - Verifica que el Host y Puerto sean correctos.
  - Asegúrate de que tu IP tenga acceso al MySQL de Railway.

### Más errores comunes

Consulta [99_Troubleshooting_FAQ.md](99_Troubleshooting_FAQ.md) para soluciones detalladas sobre:

- Conexión a CloudBeaver
- Campos faltantes en migración
- Nombres de bases de datos incorrectos
- Limpieza de DBs obsoletas
