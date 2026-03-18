# 1. Fix Manzanos y Cobranzas - Cambios

**Sprint:** 04
**Tipo:** Change
**Fecha:** 20/02/2026
**Módulo:** Inmuebles, Facturación

## Descripción

Se ajustaron los endpoints de creación y actualización de Manzanos para inferir el Condominio desde el header `X-Condominio-Id` en lugar del cuerpo de la petición. Se corrigió también el servicio de Cobranza para actualizar correctamente el saldo del `Banco` destino cuando se registra un pago.

## Cambios en DTOs

### `CreateManzanoDto` (Nuevo)

| Campo          | Cambio        | Descripción                                                             |
| -------------- | ------------- | ----------------------------------------------------------------------- |
| `IdCondominio` | **Eliminado** | Se infiere ahora a nivel de controlador por medio de `X-Condominio-Id`. |

**Ubicación:** `src/Consulcon.Application/DTOs/Inmuebles/CreateManzanoDto.cs`

## Endpoints Modificados

### 1. Crear Manzano

| Propiedad             | Valor                                                                      |
| --------------------- | -------------------------------------------------------------------------- |
| **Método**            | `POST`                                                                     |
| **Ruta**              | `api/Manzano`                                                              |
| **Descripción**       | Creación de un Manzano infiriendo el Condominio del Header                 |
| **Body / Parámetros** | Se reemplazó `ManzanoDto` por `CreateManzanoDto` omitiendo `IdCondominio`. |

### 2. Actualizar Manzano

| Propiedad             | Valor                                                                                |
| --------------------- | ------------------------------------------------------------------------------------ |
| **Método**            | `PUT`                                                                                |
| **Ruta**              | `api/Manzano/{id}`                                                                   |
| **Descripción**       | Actualización de un Manzano con validación de seguridad de contexto                  |
| **Body / Parámetros** | Se reemplazó `ManzanoDto` por `CreateManzanoDto`. Requiere header `X-Condominio-Id`. |

## Cambios en Lógica de Negocio

- **Servicio Afectado**: `ManzanoService`
- **Nuevos Cambios**: `CreateAsync` y `UpdateAsync` ahora reciben un `CreateManzanoDto` sin `IdCondominio`, obteniendo el `condominioId` por separado. Se añadió validación de seguridad en `UpdateAsync` garantizando que el Manzano a actualizar pertenece al condominio seleccionado.
- **Flujo de Datos**: El controlador `ManzanoController` extrae el `condominioId` directamente de los headers de la petición y lo envía al servicio.

- **Servicio Afectado**: `CobranzaService`
- **Nuevos Cambios**: En `RegistrarCobranzaAsync`, se modificó la validación del banco para obtener la entidad entera en lugar de validar sólo su existencia pasivamente.
- **Flujo de Datos**: Se sumó el monto del cobro (`request.Monto`) de manera directa al `banco.Saldo` antes de efectuar el commit de la transacción en Base de Datos.

## Impacto

- Cualquier cliente en conexión con la API (ej. Postman o la WebApp) no debe enviar `IdCondominio` en el `[FromBody]` en los endpoints relativos a Manzanos. A cambio, proveer el acceso a través del Header.
- Los saldos de las cuentas bancarias configuradas se inflarán acordemente con los ingresos registrados.

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02 - Postman/postman_collection.json`  
**Carpeta:** `Inmuebles > Manzano`

### Request 1: Crear Manzano (Modificado)

| Campo                | Valor                                                                      |
| -------------------- | -------------------------------------------------------------------------- |
| **Nombre**           | Crear Manzano                                                              |
| **Método**           | `POST`                                                                     |
| **URL**              | `{{baseUrl}}/api/Manzano`                                                  |
| **Headers**          | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}` |
| **Cambio Principal** | Se eliminó `idCondominio` del Body JSON                                    |

### Request 2: Actualizar Manzano (Modificado)

| Campo                | Valor                                                                      |
| -------------------- | -------------------------------------------------------------------------- |
| **Nombre**           | Actualizar Manzano                                                         |
| **Método**           | `PUT`                                                                      |
| **URL**              | `{{baseUrl}}/api/Manzano/{{manzanoId}}`                                    |
| **Headers**          | `Authorization: Bearer {{authToken}}`, `X-Condominio-Id: {{condominioId}}` |
| **Cambio Principal** | Se eliminó `idCondominio` del Body JSON                                    |
