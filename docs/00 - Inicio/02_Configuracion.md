# Configuración del Entorno

El sistema utiliza **variables de entorno** para la configuración sensible. Esta guía explica cómo configurar el entorno correctamente.

## Archivos de Configuración

| Archivo                    | Propósito                   | ¿En Git? |
| -------------------------- | --------------------------- | -------- |
| `.env`                     | Variables de entorno reales | ❌ No    |
| `.env.example`             | Plantilla de variables      | ✅ Sí    |
| `appsettings.json`         | Config local de desarrollo  | ❌ No    |
| `appsettings.json.example` | Plantilla de appsettings    | ✅ Sí    |

## Setup Inicial

### 1. Crear archivo `.env`

```powershell
Copy-Item .env.example .env
```

### 2. Crear archivo `appsettings.json` (solo desarrollo local)

```powershell
Copy-Item src/Consulcon.API/appsettings.json.example src/Consulcon.API/appsettings.json
```

### 3. Editar los valores

Modifica `.env` y/o `appsettings.json` con tus credenciales reales.

---

## Variables de Entorno

### Base de Datos (Requeridas)

| Variable           | Descripción               | Ejemplo                     |
| ------------------ | ------------------------- | --------------------------- |
| `DB_HOST`          | Host del servidor MySQL   | `db` (Docker) o `localhost` |
| `DB_PORT`          | Puerto MySQL              | `3306`                      |
| `DB_NAME`          | Base de datos por defecto | `db_consulcon_master`       |
| `DB_USER`          | Usuario MySQL             | `root`                      |
| `DB_PASSWORD`      | Contraseña MySQL          | `tu_password`               |
| `DB_ROOT_PASSWORD` | Password root (Docker)    | `tu_password`               |

### JWT Authentication (Producción)

| Variable             | Descripción                  | Ejemplo                          |
| -------------------- | ---------------------------- | -------------------------------- |
| `JWT_SECRET`         | Clave secreta (min 32 chars) | `clave-segura-32-caracteres-min` |
| `JWT_ISSUER`         | Emisor del token             | `ConsulconAPI`                   |
| `JWT_AUDIENCE`       | Audiencia del token          | `ConsulconClient`                |
| `JWT_EXPIRY_MINUTES` | Duración del token           | `60`                             |

### Docker

| Variable         | Descripción              | Ejemplo        |
| ---------------- | ------------------------ | -------------- |
| `CONTAINER_NAME` | Nombre del contenedor DB | `consulcon_db` |

---

## Prioridad de Configuración

El sistema carga la configuración en este orden (la primera encontrada gana):

1. **Variables de Entorno** (`DB_HOST`, `DB_USER`, etc.)
2. **appsettings.json** → `ConnectionStrings:DefaultConnection`
3. **Error** si ninguna está configurada

> [!IMPORTANT]
> En producción, **siempre** configura las variables de entorno. No confíes en `appsettings.json`.

---

## Verificación

Para verificar que la configuración es correcta:

```powershell
# Levantar servicios
docker-compose up -d

# Ver logs de la API
docker logs consulcon_api

# Deberías ver:
# [DI] Configuring DB Connection: Host=db, Port=3306, DB=db_consulcon_master, User=root
```

Si ves errores de configuración, revisa que `.env` exista y tenga valores correctos.
