# 02. Sistema Global de Paginación, Búsqueda y Filtros

**Sprint:** 3  
**Tipo:** Refactor / Performance  
**Fecha:** 24/02/2026

## Visión General

Esta funcionalidad estandariza el manejo de grandes volúmenes de datos en la plataforma mediante la implementación del patrón **Specification**. Se ha migrado la lógica de listados pesados de "Carga en Memoria" a "Paginación en Base de Datos", reduciendo drásticamente el consumo de recursos y mejorando la experiencia de usuario (UX) con capacidades de búsqueda y ordenamiento dinámico.

## Arquitectura de Paginación

### 1. Parámetros de Consulta (`PaginationParams`)
- **Ubicación**: `src/Consulcon.Domain/Common/PaginationParams.cs`
- **Propósito**: Objeto estandarizado para recibir:
  - `PageNumber` / `PageSize`: Control de registros por página.
  - `SearchTerm`: Texto para búsqueda global (NIT, Razón Social, Código Unidad, etc.).
  - `SortBy` / `SortDescending`: Control de ordenamiento dinámico.
  - `FromDate` / `ToDate`: Filtros de rango temporal.

### 2. Patrón Specification (Lógica de Filtrado)
- **Ubicación**: `src/Consulcon.Domain/Specifications/`
- **Implementaciones**:
  - `ExpenseWithFiltersSpec`: Filtra egresos por proveedor, fecha y concepto.
  - `CobranzaWithFiltersSpec`: Filtra pagos por unidad, fechas y estado.
  - `PropiedadWithFiltersSpec`: Filtra unidades por condominio y código.
  - `TransactionHistoryWithFiltersSpec`: Filtra historial contable por cuenta bancaria.



## Servicios Actualizados

Se ha inyectado `IRepository<T>` en los servicios core para habilitar el motor de paginación:

- **`ExpenseService`**: Paginación de egresos con navegación hacia proveedores.
- **`CobranzaService`**: Implementación de lógica compleja para aplanar cobros relacionados a facturas.
- **`AccountService`**: Nuevo motor de historial contable paginado usando `BalanceHistoryDto`.
- **`PropiedadService`**: Listado optimizado de unidades con cálculo de saldo deudor.
- **`ProveedorService`**: Búsqueda optimizada por términos de texto.

## Endpoints (API)

Todos los endpoints paginados devuelven un objeto `PagedResult<T>` que incluye la data y los metadatos de paginación (`TotalCount`, `TotalPages`, `HasNextPage`).

| Módulo | Método | Endpoint | Filtros Soportados |
| :--- | :--- | :--- | :--- |
| **Egresos** | `GET` | `/api/egresos/paged` | Fecha, Proveedor, Concepto |
| **Cobranzas** | `GET` | `/api/cobranzas/condominio/{id}/paged` | Rango de fechas, Referencia |
| **Cuentas** | `GET` | `/api/accounts/{id}/transacciones/paged` | Rango de fechas, Descripción |
| **Propiedades** | `GET` | `/api/propiedades/condominio/{id}/paged` | Código Unidad, Saldo, Nombre |
| **Proveedores** | `GET` | `/api/proveedores/paged` | NIT, Razón Social |

> [!IMPORTANT]
> Los endpoints requieren el header `X-Condominio-Id`. El parámetro `idCondominio` en la URL prevalece para la seguridad del filtrado de datos.

## Control de Calidad (Tests)

Se han actualizado los constructores de los servicios en el proyecto de pruebas de integración para soportar la nueva inyección de dependencias de repositorios genéricos:

- **Ubicación**: `tests/Consulcon.IntegrationTests/Services/`
- **Archivos Modificados**: `AccountServiceTests.cs`, `CobranzaServiceTests.cs`, `ExpenseServiceTests.cs`.
- **Estado**: Build exitoso (Compilación en verde).

### Ejemplo de Paginación (Postman)

**Request: Get Paged Transactions**
- **URL**: `{{baseUrl}}/api/accounts/1/transacciones/paged?pageNumber=1&pageSize=10&searchTerm=Expensa&sortBy=date&sortDescending=true`
- **Respuesta esperada**:
  ```json
  {
    "items": [...],
    "pageNumber": 1,
    "totalPages": 5,
    "totalCount": 48,
    "hasPreviousPage": false,
    "hasNextPage": true
  }