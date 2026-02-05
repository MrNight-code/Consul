# Guía Técnica de Migración de Base de Datos

Esta guía detalla el proceso automatizado para migrar bases de datos "Legacy" al nuevo esquema de **Consulcon**. El sistema soporta migraciones tanto a **entornos locales (Docker)** como a **bases de datos en la nube**.

## 1. Arquitectura de Migración

El script `Import-LegacyDatabase.ps1` orquesta todo el proceso creando un entorno controlado (Staging) para transformar los datos sin riesgos.

### Flujo de Datos Automatizado

1.  **Staging DB**: Se crea una base de datos temporal (ej: `temp_import_bosques`) para cargar el dump original crudo.
2.  **Schema Initialization**: Se utiliza el contenedor de la API (`--migrate-only`) para crear la estructura de tablas vacía en la base de datos destino.
3.  **Transformation**: Se ejecuta el script SQL de migración que mueve y transforma los datos desde Staging hacia la DB destino.

## 2. Instrucciones de Uso

### Prerrequisitos

- Docker Desktop corriendo.
- Powershell.
- El archivo `.sql` de la base de datos antigua.

### Comando de Migración

Desde la raíz del proyecto (`Backend-Consulcon`), ejecute el script:

```powershell
.\scripts\utils\Import-LegacyDatabase.ps1 `
    -SourceSqlDump "scripts\database\data\Bosques\syscons1_bdbosquescolina.sql" `
    -StagingDbName "temp_import_bosques" `
    -MigrationScript "scripts\database\migrations\Bosques\migrated_bosques_colina.sql" `
    -TargetDbName "db_condominio_bosques"
```

#### Parámetros Opcionales para Nube/Remoto

Por defecto, el script asume que todo corre en el Docker local (`127.0.0.1`). Para migrar hacia una base de datos externa o en la nube, use los siguientes parámetros extra:

- `-DbHost`: Dirección IP o Host de la base de datos (Ej: `mi-db.aws.com`).
- `-DbPort`: Puerto (Defecto: `3306`).
- `-DbUser`: Usuario con permisos de creación de DB.
- `-DbPassword`: Contraseña del usuario.
- `-UseDockerExec`: `$false` (Importante para conectarse a una DB que no es el contenedor local).

**Ejemplo Nube:**

```powershell
.\Import-LegacyDatabase.ps1 ... -TargetDbName "db_condominio_prod" -DbHost "aws-rds-url..." -DbUser "admin" -UseDockerExec $false
```

---

## 3. Generación de Scripts de Transformación (`generate_migration.py`)

Si es un **nuevo condominio**, primero debe generar el script SQL de mapeo:

```bash
python scripts/database/migrations/python_scripts/generate_migration.py --source-db "{{STAGING_DB}}" --target-db "{{TARGET_DB}}" --output-name "migrated_nuevo_condo.sql"
```

_Nota: Mantenga `{{STAGING_DB}}` y `{{TARGET_DB}}` literal; el script de PowerShell los reemplazará dinámicamente._

## 4. Solución de Problemas Comunes

### 📅 Fechas Invalidas

El script maneja automáticamente fechas `0000-00-00` convirtiéndolas a `NULL`.

### 🚫 Schema faltante

Si obtiene error de "Table not found", asegúrese que el paso "Initializing Target DB Scheme" del script se ejecutó correctamente (debe decir "Schema initialized successfully").

### 🔒 Conexión Rechazada (Nube)

Asegúrese que su IP esté en la lista blanca (Security Group) de la base de datos en la nube para permitir la conexión en el puerto 3306.

---

## 5. Ver También

- [Troubleshooting y FAQ Completo](03_Troubleshooting_FAQ.md) - Soluciones detalladas a errores comunes
- [Caso Práctico: Bosques](02_Caso_Practico_Bosques.md) - Ejemplo real paso a paso
