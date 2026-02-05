# Guía de Despliegue en la Nube (Railway)

Esta guía explica cómo desplegar el Backend de **Consulcon** (.NET 9 API + MySQL) en un proveedor de nube moderno. Recomendamos **Railway.app** por su simplicidad y soporte nativo para Docker y MySQL, pero los principios aplican a Render, Azure App Service, o AWS App Runner.

## 1. Preparación del Repositorio

Asegúrate de que tu código esté actualizado en GitHub.

- El repositorio debe tener el `Dockerfile` en el lugar correcto (`src/Consulcon.API/Dockerfile`).
- El archivo `docker-compose.yml` ayuda al desarrollo local, pero en la nube configuraremos los servicios individualmente o mediante un archivo de configuración propio de la plataforma.

## 2. Configuración en Railway (Paso a Paso)

### Paso A: Crear Proyecto y Base de Datos

1.  Inicia sesión en [Railway.app](https://railway.app/).
2.  Haz clic en **"New Project"**.
3.  Selecciona **"Provision MySQL"**.
    - Esto creará automáticamente un contenedor con MySQL listo para usar.
4.  Una vez creado, haz clic en el servicio MySQL y ve a la pestaña **"Variables"** o **"Connect"**.
    - Anota los valores de: `MYSQLHOST`, `MYSQLPORT`, `MYSQLUSER`, `MYSQLPASSWORD`, `MYSQLDATABASE`.

### Paso B: Importar el Backend (API)

1.  En el mismo proyecto, haz clic en **"New"** -> **"GitHub Repo"**.
2.  Selecciona el repositorio de **Backend-Consulcon**.
3.  Railway detectará automáticamente el `Dockerfile`.
4.  Antes de que termine de desplegar (o si falla al principio), ve a la pestaña **"Variables"** del servicio de tu API.

### Paso C: Configurar Variables de Entorno (API)

Agrega las siguientes variables en Railway para el servicio del Backend:

| Variable                 | Valor (Ejemplo / Origen)                                                                             |
| :----------------------- | :--------------------------------------------------------------------------------------------------- |
| `ASPNETCORE_ENVIRONMENT` | `Production`                                                                                         |
| `DB_HOST`                | `${{MySQL.MYSQLHOST}}` (Railway permite usar variables de otros servicios)                           |
| `DB_PORT`                | `${{MySQL.MYSQLPORT}}`                                                                               |
| `DB_USER`                | `${{MySQL.MYSQLUSER}}`                                                                               |
| `DB_PASSWORD`            | `${{MySQL.MYSQLPASSWORD}}`                                                                           |
| `JwtSettings__Secret`    | Genera una cadena larga y segura alfanumérica.                                                       |
| `PORT`                   | `8080` (Opcional, Railway suele detectarlo, pero .NET escucha en 8080 por defecto en nuestra imagen) |

_Nota: Al usar variables de referencia como `${{MySQL.MYSQLHOST}}`, Railway mantiene sincronizados los servicios si las IPs cambian._

### Paso D: Networking (Dominios)

1.  Ve a la pestaña **"Settings"** de tu servicio API.
2.  En **"Networking"**, haz clic en **"Generate Domain"**.
3.  Obtendrás una URL pública (ej: `backend-consulcon-production.up.railway.app`) que podrás usar desde tu Frontend.

## 3. Migración de Base de Datos a la Nube

Una vez que la API y la Base de Datos están corriendo, la base de datos estará vacía. Debemos ejecutar nuestro script de migración apuntando a esta nueva infraestructura.

### Desde tu máquina local:

1.  Obtén las credenciales públicas de tu base de datos en Railway (Pestaña "Connect" -> Datos de conexión pública).
2.  Ejecuta el script `Import-LegacyDatabase.ps1` con la bandera `UseDockerExec` desactivada:

```powershell
.\scripts\utils\Import-LegacyDatabase.ps1 `
    -SourceSqlDump "scripts\database\data\Bosques\syscons1_bdbosquescolina.sql" `
    -StagingDbName "db_temp_import" `
    -MigrationScript "scripts\database\migrations\Bosques\migrated_bosques_colina.sql" `
    -TargetDbName "db_condominio_bosques_colina" `
    -DbHost "viaduct.proxy.rlwy.net" `
    -DbPort "12345" `
    -DbUser "root" `
    -DbPassword "tupasswordsecreto" `
    -UseDockerExec $false
```

- **¿Qué hace esto?**
  - Crea una base de datos temporal en TU Docker local.
  - Procesa los datos.
  - Se conecta a la nube (Railway) para inicializar el esquema.
  - Envía los datos procesados a la nube.