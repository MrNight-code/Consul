# Guía Rápida: Migración Local (Docker)

Esta guía te permite migrar una base de datos legacy a tu entorno Docker local en **3 pasos**.

---

## 📋 Pre-requisitos

1. Docker Desktop corriendo
2. Repositorio clonado y ubicado en la raíz del proyecto
3. Archivo `.env` configurado (puedes copiar `.env.example`)

---

## 🚀 Pasos para Migración Local

### Paso 1: Levantar la Infraestructura

```powershell
docker-compose up -d
```

Esto iniciará:

- **MySQL** (contenedor `consulcon_db`)
- **API** (contenedor `consulcon_api`)
- **CloudBeaver** (opcional, para visualizar BD)

Espera ~10 segundos para que MySQL termine de inicializar.

---

### Paso 2: Ejecutar Script de Migración

Desde PowerShell, ejecuta el script `Import-LegacyDatabase.ps1`.

> **Nota sobre Puertos:**
> El script está configurado para manejar automáticamente los puertos.
>
> - Para operaciones internas (Docker), usa el puerto estándar `3306`.
> - Para herramientas locales que corren en el host (como `PasswordHasher`), usa el puerto mapeado `3310` definido en `docker-compose.yml`.
>   No necesitas cambiar el parámetro `-DbPort` a menos que tu configuración de Docker sea diferente.

#### Para migrar "Bosques Colina":

```powershell
.\scripts\utils\Import-LegacyDatabase.ps1 `
    -SourceSqlDump "scripts\database\data\Bosques\syscons1_bdbosquescolina.sql" `
    -StagingDbName "db_temp_bosques" `
    -MigrationScript "scripts\database\migrations\Bosques\migrated_bosques_colina.sql" `
    -TargetDbName "db_condominio_bosques_colina"
```

**¿Qué hace este comando?**

1. Crea una BD temporal (`db_temp_bosques`) y carga el dump legacy
2. Ejecuta el script de transformación SQL
3. Crea automáticamente la BD destino (`db_condominio_bosques_colina`) usando la API en modo migración
4. Mueve y adapta los datos al nuevo esquema

---

### Paso 3: Verificar la Migración

**Opción A: Usando CloudBeaver (Interfaz Gráfica)**

1. Abre [http://localhost:8978](http://localhost:8978)
2. Conecta a `consulcon_db` (Host: `db`, Usuario: `root`, Password: según `.env`)
3. Navega a `db_condominio_bosques_colina` y revisa las tablas

**Opción B: Desde PowerShell**

```powershell
docker exec -it consulcon_db mysql -uroot -proot -e "USE db_condominio_bosques_colina; SHOW TABLES;"
```

---

## 🔧 Migrar Otro Condominio

Para migrar un condominio diferente (ej: "Foret"), solo cambia los parámetros:

```powershell
.\scripts\utils\Import-LegacyDatabase.ps1 `
    -SourceSqlDump "scripts\database\data\Foret\dump_legacy_foret.sql" `
    -StagingDbName "db_temp_foret" `
    -MigrationScript "scripts\database\migrations\Foret\migrated_foret.sql" `
    -TargetDbName "db_condominio_foret"
```

> **Importante:** Cada condominio tiene su propia Base de Datos. El sistema multi-tenant funciona creando BDs separadas por condominio.

---

## ❓ Troubleshooting

### Error: "No such file or directory"

**Causa:** Rutas incorrectas.
**Solución:** Asegúrate de estar en la raíz del proyecto (`Backend-Consulcon/`) antes de ejecutar el comando.

### Error: "Container consulcon_db not found"

**Causa:** Docker no está corriendo o los contenedores no se levantaron.
**Solución:**

```powershell
docker-compose down
docker-compose up -d
```

### Error: "Access denied for user 'root'"

**Causa:** Contraseña incorrecta en `.env`.
**Solución:** Verifica que `DB_ROOT_PASSWORD` en `.env` coincida con el parámetro `-DbPassword` (por defecto es `root`).

### La API no arranca después de migración

**Causa:** La cadena de conexión en `appsettings.json` o el `.env` no es correcta.
**Solución:**

- El sistema ahora detecta automáticamente si estás en Docker o Local.
- Asegúrate de tener `DB_NAME` definido en el `.env` si no estás usando un `TenantId`.
- Para desarrollo local fuera de Docker contra el contenedor, usa el puerto `3310`. El script de migración maneja esto automáticamente para sus utilidades.

---

## 🛠️ Detalles Técnicos de Conexión

Hemos mejorado la API para que la configuración sea más robusta:

1. **Prioridad de Variables de Entorno:** Los valores de `DB_HOST`, `DB_PORT`, `DB_USER` y `DB_PASSWORD` en el `.env` siempre sobreescriben a `appsettings.json`.
2. **Inyección Dinámica de Base de Datos:**
   - Si se provee un `X-Tenant-Id` en el Header, la API buscará la BD `db_condominio_{TenantId}`.
   - Si el `TenantId` ya empieza con `db_`, se usa tal cual.
   - Esto permite que el script de migración inicialice el esquema directamente sin configuraciones manuales complejas.
3. **Persistencia de Parámetros:** Se mantienen parámetros críticos como `TreatTinyAsBoolean=true` de la cadena por defecto.

---

## 📚 Documentación Adicional

- **[Guía Técnica Completa](./01_Guia_Tecnica_Migracion.md):** Detalles sobre la arquitectura de migración y cómo funciona el script Python.
- **[Migración a la Nube](../../03%20-%20Despliegue/01_Guia_Despliegue_Cloud.md):** Pasos para migrar directamente a Railway/AWS.
