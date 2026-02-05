# Guía de Inicio y Setup

Esta guía explica detalladamente cómo estructurar, iniciar y gestionar el entorno de desarrollo del sistema Consulcon.

## 1. Requisitos Previos

- **Docker Desktop**: Para levantar Base de Datos y API fácilmente.
- **.NET 8 SDK**: Si planeas ejecutar o debugear el backend localmente.
- **DBeaver** (o similar): Para gestionar la base de datos.
- **PowerShell**: Recomendado para ejecutar scripts de utilidad.

## 2. Ejecución Rápida con Docker (Recomendado)

La forma más sencilla de iniciar el sistema es utilizando Docker Compose. Esto levantará la API, la Base de Datos y herramientas auxiliares.

### Pasos:

1.  **Configurar Entorno**:

    - Copia el archivo `.env.example` a `.env`.
    - Ajusta las credenciales si es necesario (por defecto `root`/`root`).

2.  **Iniciar Servicios**:
    Desde la raíz del proyecto, ejecuta:

    ```powershell
    docker-compose up -d
    ```

3.  **Verificar**:
    - **API**: [http://localhost:5000/swagger](http://localhost:5000/swagger) (Swagger UI)
    - **CloudBeaver**: [http://localhost:8978](http://localhost:8978) (Gestor DB Web)
    - **MySQL**: Puerto `3310`

### Estructura de Volúmenes y Datos Docker

Docker montará automáticamente los scripts de esquema iniciales:

- **Schema**: `scripts/database/schema/` -> Se ejecuta al iniciar el contenedor vacío.
- **Datos**: Si deseas resetear la DB, debes eliminar el volumen: `docker volume rm backend-consulcon_mysql_data`.

## 3. Ejecución Manual (Local)

Si prefieres ejecutar el código fuente directamente (para hot-reload o debugging):

1.  **Base de Datos**: Asegúrate de que MySQL esté corriendo (puedes usar `docker-compose up -d db` para levantar solo la BD).
2.  **Ejecutar API**:
    ```powershell
    cd src/Consulcon.API
    dotnet run
    ```
3.  La API estará disponible en `http://localhost:3010` (o el puerto configurado en launchSettings).

## 4. Estructura del Proyecto

Breve descripción de la organización de carpetas:

- **`src/`**: Código fuente (Clean Architecture).
  - `API`: Entry point y controladores.
  - `Application`: Casos de uso y lógica.
  - `Domain`: Entidades core.
  - `Infrastructure`: EF Core y servicios externos.
- **`scripts/`**:
  - `database/schema`: DDL (Tablas).
  - `database/data`: Dumps de datos iniciales.
  - `utils`: Herramientas de migración (Powershell/Python).
- **`docs/`**: Documentación (esta carpeta).
