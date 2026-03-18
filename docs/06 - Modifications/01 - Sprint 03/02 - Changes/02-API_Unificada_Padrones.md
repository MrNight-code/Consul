# 02. API Unificada de Padrones: Unidades y Personas - Cambios

**Sprint:** 03
**Tipo:** Change
**Fecha:** 29/01/2026
**Módulo:** Inmuebles, Personas

## Descripción

Implementación y unificación de la gestión de padrones para Unidades (Propiedades) y Residentes (Personas). Se han añadido validaciones de unicidad crítica y se ha mejorado la estructura de datos para incluir información de contacto directa en las personas.

## Endpoints Implementados

### Personas (`/api/Persona`)

| Método   | Endpoint            | Descripción                                    |
| -------- | ------------------- | ---------------------------------------------- |
| `GET`    | `/api/Persona`      | Obtiene todas las personas registradas.        |
| `GET`    | `/api/Persona/{id}` | Obtiene una persona por su ID.                 |
| `POST`   | `/api/Persona`      | Crea una nueva persona. Valida unicidad de CI. |
| `PUT`    | `/api/Persona/{id}` | Actualiza una persona existente.               |
| `DELETE` | `/api/Persona/{id}` | Elimina (o desactiva) una persona.             |

### Propiedades (`/api/Propiedad`)

| Método   | Endpoint                         | Descripción                                                        |
| -------- | -------------------------------- | ------------------------------------------------------------------ |
| `GET`    | `/api/Propiedad`                 | Obtiene todas las propiedades (unidades).                          |
| `GET`    | `/api/Propiedad/{id}`            | Obtiene una propiedad por su ID.                                   |
| `GET`    | `/api/Propiedad/condominio/{id}` | Obtiene todas las propiedades de un condominio específico.         |
| `POST`   | `/api/Propiedad`                 | Crea una nueva unidad. Valida unicidad de Código en el Condominio. |
| `PUT`    | `/api/Propiedad/{id}`            | Actualiza una unidad existente.                                    |
| `DELETE` | `/api/Propiedad/{id}`            | Elimina una unidad.                                                |

## Cambios en Postman

Se ha actualizado la colección principal del proyecto para incluir los ejemplos de uso de estos nuevos endpoints.

- **Archivo Actualizado**: `docs/99 - Otros/02-postman/postman_collection.json`
- **Ubicación en Postman**: Se han agregado dos nuevas carpetas en la raíz de la colección:
  1.  📁 **Persona**: Contiene CRUD completo + ejemplos de JSON con `MedioContactos`.
  2.  📁 **Propiedad**: Contiene CRUD completo + busqueda por Condominio.

## Cambios en Infraestructura

### 1. Dependencias (Bug Fix - Error 500)

- **Inyección de Dependencias**: Se registraron explícitamente `IPersonaService` e `IPropiedadService` en `DependencyInjection.cs`. Antes causaba un error 500 al intentar resolver los controladores.

### 2. DTOs

- **Modificación**: `PersonaDto` ahora incluye una lista de `MedioContactoDto` para gestionar teléfonos, correos, etc. en la misma transacción.
- **Nuevo**: `MedioContactoDto`.

## Cambios en Lógica de Negocio

### 1. Servicios Afectados

#### `PersonaService`

- **Validación de Unicidad Global**: Se impide la creación o modificación de una persona si el documento de identidad (`Ci`) ya existe en la base de datos.
- **Gestión de Contactos**: `MedioContactos` se gestionan transaccionalmente con la Persona.

#### `PropiedadService`

- **Validación de Unicidad por Condominio**: Se impide duplicados de `CodigoUnidad` dentro del mismo Condominio.
- **Validación de Manzano**: Se verifica que el Manzano asignado exista y corresponda a un Condominio válido.
- **Seguridad y Robustez**: Se implementaron chequeos de nulidad (`?.` y `?? []`) para prevenir errores en tiempo de ejecución (Error 500) al mapear listas opcionales como `MedioContactos`.

## Impacto

- **Integridad de Datos**: Garantizada por las validaciones de servicio.
- **Postman**: Es necesario re-importar la colección para ver las nuevas carpetas.
