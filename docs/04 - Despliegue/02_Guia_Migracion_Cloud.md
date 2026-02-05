# Guía de Migración y Gestión de Base de Datos en la Nube (Railway)

Esta guía detalla cómo gestionar las bases de datos multi-tenant una vez que el backend está desplegado en producción (Railway).

## 1. Arquitectura de Base de Datos

El sistema utiliza **Database-per-Tenant**.

- Base de datos principal (`consulcon_db`): Metadatos, Usuarios Globales, lista de Condominios.
- Bases de datos Tenant (`db_condominio_X`): Datos específicos de cada edificio.

---

## 2. Escenarios de Migración

### A. Creación de Nuevo Condominio (Automático)

Cuando creas un nuevo condominio desde la App o Postman, el sistema hace todo el trabajo.

**Acción:**

- Enviar `POST` a `/api/Condominio`

**Lo que sucede en la nube:**

1. Se crea el registro en la DB Principal.
2. La API se conecta al servidor MySQL y ejecuta `CREATE DATABASE db_condominio_{id}`.
3. La API inicializa las tablas (Schema) usando Entity Framework.
4. La API busca scripts `.sql` en la carpeta `/app/migrations` y los ejecuta.

---

### B. Actualización de Esquema (Mantenimiento)

Si subes nuevos cambios al código (ej: nueva tabla o columna) y necesitas aplicarlos a condominios YA EXISTENTES.

**Acción:**

1. Desplegar la nueva versión del Backend en Railway (los archivos `.sql` se copian automáticamente).
2. Ejecutar el endpoint de mantenimiento para cada tenant.

**Endpoint:**
`POST https://tu-app.railway.app/api/maintenance/migrate/{id_condominio}`

**Ejemplo:**
Para actualizar la base de datos del condominio con ID 5:

```bash
POST /api/maintenance/migrate/5
```

_Esto ejecutará las migraciones pendientes sin borrar datos._

---

### C. Importación de Datos Legados (Desde tu PC a la Nube)

Si tienes un backup de un sistema antiguo (`.sql`) y quieres subirlo a un condominio específico en la nube.

**Pre-requisitos:**

1. El Condominio debe existir en la nube (creado con el paso A).
2. Necesitas la IP pública y credenciales de tu base de datos en Railway (MySQL Service).

**Herramienta:**
Usar el script local `scripts/utils/Import-LegacyDatabase.ps1`.

**Comando:**

```powershell
.\scripts\utils\Import-LegacyDatabase.ps1 `
    -SourceSqlDump "scripts\database\data\Bosques\syscons1_bdbosquescolina.sql" `
    -StagingDbName "db_temp_bosques" `
    -MigrationScript "scripts\database\migrations\Bosques\migrated_bosques_colina.sql" `
    -TargetDbName "db_condominio_bosques_colina" `
    -DbHost "monorail.proxy.rlwy.net" `
    -DbPort "12345" `
    -DbUser "root" `
    -DbPassword "tu_password_railway" `
    -UseDockerExec $false
```

> **Importante:**
>
> - Reemplaza `DbHost`, `DbPort` y `DbPassword` con tus valores reales de Railway.
> - El parámetro `-UseDockerExec $false` es crítico para conectarse remotamente (en lugar del contenedor Docker local).

---

## 3. Comandos de Utilidad

### Listar todas las bases de datos en Railway

```bash
mysql -h monorail.proxy.rlwy.net -P 12345 -u root -p -e "SHOW DATABASES;"
```

### Borrar una DB de tenant (CUIDADO)

```sql
DROP DATABASE IF EXISTS db_condominio_5;
```

---

## 4. Solución de Problemas

### Error: "Unknown database"

- **Causa:** La base de datos no fue creada por la API.
- **Solución:** Usa Postman para crear el condominio (`POST /api/Condominio`) antes de migrar datos.

### Error de conexión a Railway

- **Causa:** Credenciales incorrectas o firewall.
- **Solución:**
  - Verifica que el Host y Puerto sean correctos.
  - Asegúrate de que tu IP esté en la whitelist de Railway (si está habilitada).

### La migración es muy lenta

- **Causa:** Railway usa conexiones proxy, la latencia puede ser alta.
- **Solución:** Considera ejecutar el backup desde un servidor en la nube cercano a Railway.
