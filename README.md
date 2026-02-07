# Sistema Gabriel - Backend Consulcon

Bienvenido al backend del Sistema Gabriel (Consulcon). Este proyecto ha sido reestructurado para facilitar su mantenimiento y escalabilidad.

## Estructura del Proyecto

El código base está organizado de la siguiente manera:

- **`src/`**: Contiene todo el código fuente del backend (API, Aplicación, Dominio, Infraestructura).
- **`scripts/`**: Scripts de utilidad y base de datos.
  - `scripts/database/`: Scripts SQL para esquema, datos iniciales y migraciones.
- **`docs/`**: Documentación detallada del proyecto.

## Build y Archivos Generados (.gitignore)

Es posible que notes que las carpetas `bin/` y `obj/` no aparecen en el repositorio. Esto es **intencional**.

### ¿Por qué se ignoran?

- **`bin/` (Binarios)**: Contiene el resultado final de la compilación (`.dll`, `.exe`).
- **`obj/` (Objetos)**: Contiene archivos temporales utilizados durante la compilación.

Estos archivos se generan automáticamente, son pesados y específicos de cada máquina, por lo que nunca deben subirse a Git.

### ¿Cómo regenerarlos?

Solo necesitas usar los comandos de .NET CLI. El framework los creará por ti:

```powershell
# 1. Restaurar dependencias (descarga librerías)
dotnet restore

# 2. Compilar el proyecto (genera bin/ y obj/)
dotnet build

# 3. Ejecutar la aplicación
dotnet run --project src/Consulcon.API
```

## Documentación

Toda la documentación se encuentra en la carpeta `docs/` y está organizada por temas:

1.  **[00 - Inicio](docs/00%20-%20Inicio/setup.md)**: Guía de configuración e inicio rápido (Docker y Local).
2.  **[01 - Estructura](docs/01%20-%20Estructura/estructura.md)**: Explicación detallada de la arquitectura de archivos.
3.  **[02 - Base de Datos](docs/02%20-%20Base%20de%20Datos/base_de_datos.md)**: Información sobre el esquema de base de datos y scripts.
4.  **[99 - Otros](docs/99%20-%20Otros)**: Recursos adicionales (como la colección de Postman).

## Inicio Rápido

Para iniciar el proyecto con Docker:

```bash
docker-compose up -d --build
```

Esto levantará la API y la base de datos MySQL inicializada automáticamente.

Para más detalles, consulta la guía de **[Inicio](docs/00%20-%20Inicio/setup.md)**.
