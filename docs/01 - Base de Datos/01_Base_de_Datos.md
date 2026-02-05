# Manual de Base de Datos

Documentación completa sobre el diseño, scripts y gestión de la base de datos `db_condominio` y `db_consulcon_master`.

Documentación relacionada con el diseño y migración de la base de datos.

## Estructura de Scripts

Para facilitar la inicialización con Docker, mantenemos una distinción clara entre **Schema** y **Datos**:

### 1. Schema (`scripts/database/schema/01_schema.sql`)

Este archivo contiene la definición DDL de la base de datos:

- `CREATE TABLE`
- `ALTER TABLE` (Llaves foráneas, índices)
- Stored Procedures o Functions iniciales.

### 2. Datos Migrados (`scripts/database/schema/02_data_migration.sql`)

Este archivo contiene los datos extraídos del sistema anterior/legacy que serán usados en este entorno:

- `INSERT INTO` masivos.
- Datos de catálogos necesarios.

## Flujo de Trabajo

Cualquier cambio en la estructura debe ser reflejado en un script de migración incremental o actualizando el `01_schema.sql` si estamos en etapas tempranas de desarrollo (antes de producción). Wl entorno Docker ejecutará automáticamente los scripts en orden alfabético.

---

# Base de Datos Maestra (`db_consulcon_master`)

## Propósito General

La **Base de Datos Maestra** es el componente central de la arquitectura multi-tenant de Consulcon. Su objetivo principal es:

1.  **Centralizar la identidad de los usuarios**: Permite un único conjunto de credenciales para acceder a múltiples condominios (tenants).
2.  **Discovery de Tenants**: Almacena qué usuarios tienen acceso a qué condominios.
3.  **Configuración Global**: Guarda la cadena de conexión y metadatos de cada tenant.

## Esquema de Base de Datos

### 1. Tabla `CondominiosMaster`

Registro de todos los condominios registrados en el sistema.

| Columna            | Tipo         | Descripción                                                                                                                      |
| :----------------- | :----------- | :------------------------------------------------------------------------------------------------------------------------------- |
| `Id`               | INT (PK)     | Identificador único del condominio en la master.                                                                                 |
| `TenantId`         | VARCHAR(50)  | Identificador único tipo slug (ej: `bosques`). Se usa para la DB del tenant (`db_condominio_bosques`) y headers (`X-Tenant-Id`). |
| `Nombre`           | VARCHAR(100) | Nombre legible del condominio.                                                                                                   |
| `ConnectionString` | VARCHAR(500) | (Opcional) Cadena de conexión específica si difiere del servidor por defecto.                                                    |
| `FechaCreacion`    | DATETIME     | Fecha de registro.                                                                                                               |

### 2. Tabla `UsuariosMaster`

Usuarios globales del sistema. Un usuario aquí puede tener acceso a N condominios.

| Columna        | Tipo         | Descripción                                         |
| :------------- | :----------- | :-------------------------------------------------- |
| `Id`           | INT (PK)     | Identificador único global.                         |
| `Username`     | VARCHAR(100) | Nombre de usuario único.                            |
| `PasswordHash` | VARCHAR(255) | Hash de contraseña (BCrypt).                        |
| `Email`        | VARCHAR(150) | Correo electrónico de recuperación/contacto.        |
| `EsSuperAdmin` | BOOLEAN      | Indica si tiene permisos globales de mantenimiento. |

### 3. Tabla `UsuarioCondominio`

Tabla pivote que relaciona usuarios con condominios.

| Columna        | Tipo     | Descripción                       |
| :------------- | :------- | :-------------------------------- |
| `UsuarioId`    | INT (FK) | Referencia a `UsuariosMaster`.    |
| `CondominioId` | INT (FK) | Referencia a `CondominiosMaster`. |

## Relación con Bases de Datos de Tenant

Cada tenant tiene su propia base de datos (`db_condominio_{TenantId}`).

- La tabla `Usuario` dentro de la DB del Tenant debe tener una referencia (lógica o campo) al usuario global para mantener la coherencia, aunque funcionalmente `AuthService` valida primero contra Master y luego obtiene roles del Tenant.

## Inicialización

La base de datos maestra se inicializa automáticamente al arrancar la API mediante `DatabaseMigrationInitializer`.

- Si no existe ningún usuario, se crea automáticamente el usuario `admin` con contraseña `admin123`.

---

# Guía de Ejecución de Scripts (DBeaver/MySQL)

A continuación se detalla cómo utilizar los scripts SQL proporcionados para gestionar la base de datos manualmente.

## Estructura de Scripts (`scripts/database/`)

- **Schema (`/schema`)**: Define las tablas y relaciones (`01_schema.sql`).
- **Data (`/data`)**: Datos iniciales y dumps legacy.
- **Migrations (`/migrations`)**: Scripts de transformación.

## Ejecución con DBeaver

Si necesitas reiniciar la base de datos o correr scripts manualmente:

1.  **Conexión**:

    - Host: `localhost`
    - Puerto: `3310` (Docker)
    - User/Pass: `root` / `root` (ver `.env`)

2.  **Cargar Schema**:

    - Abre `scripts/database/schema/sysconsu_asai2_schema.sql`.
    - Ejecuta el script completo (Alt+X).

3.  **Cargar Datos**:
    - Abre los scripts de `scripts/database/data/`.
    - Ejecuta en orden si hay numeración.

> **Tip**: Si usas Docker y quieres un inicio limpio, es más fácil borrar el volumen `consulcon_mysql_data` y reiniciar el contenedor; el entrypoint ejecutará los scripts de schema automáticamente.
