# 09. API de Condominios y Gestión de Usuarios

**Sprint:** 03  
**Tipo:** Change  
**Fecha:** 06/02/2026

---

## Visión General

Se refactorizó la API de Condominios para simplificar la respuesta del DTO eliminando campos redundantes (`codigo`, `tenantId`). Adicionalmente, se implementó un CRUD completo para gestionar los usuarios asignados a cada condominio, permitiendo listar, agregar y eliminar usuarios.

---

## Cambios en el DTO

### `CondominioDto` (Modificado)

| Campo      | Cambio        | Descripción                                       |
| ---------- | ------------- | ------------------------------------------------- |
| `codigo`   | **Eliminado** | Era redundante con `TenantId`.                    |
| `tenantId` | **Eliminado** | Es un identificador interno, no necesario en API. |

**Ubicación:** `src/Consulcon.Application/DTOs/Inmuebles/CondominioDto.cs`

### `AddUserToCondominioDto` (Modificado)

| Campo        | Cambio        | Descripción                                    |
| ------------ | ------------- | ---------------------------------------------- |
| `email`      | **Eliminado** | Ya no se busca por email.                      |
| `rolInicial` | **Eliminado** | Ahora se asigna rol fijo "Usuario".            |
| `userId`     | **Nuevo**     | ID del usuario Master a asignar al condominio. |

**Ubicación:** `src/Consulcon.Application/DTOs/Inmuebles/AddUserToCondominioDto.cs`

---

## Nuevos DTOs

### `CondominioUserDto`

| Campo        | Tipo     | Descripción               |
| ------------ | -------- | ------------------------- |
| `UserId`     | `int`    | ID del usuario en Master. |
| `Username`   | `string` | Nombre de usuario.        |
| `FullName`   | `string` | Nombre completo.          |
| `Email`      | `string` | Correo electrónico.       |
| `RolInicial` | `string` | Rol inicial asignado.     |

**Ubicación:** `src/Consulcon.Application/DTOs/Inmuebles/CondominioUserDto.cs`

---

## Controller

**Ubicación:** `src/Consulcon.API/Controllers/Inmuebles/CondominioController.cs`  
**Ruta Base:** `api/Condominio`

---

## Endpoints

### 1. Listar Usuarios del Condominio

| Propiedad       | Valor                                             |
| --------------- | ------------------------------------------------- |
| **Método**      | `GET`                                             |
| **Ruta**        | `/api/Condominio/{id}/usuarios`                   |
| **Descripción** | Lista todos los usuarios asignados al condominio. |
| **Parámetros**  | `id` (int): ID del condominio.                    |
| **Respuesta**   | `IEnumerable<CondominioUserDto>`.                 |

### 2. Agregar Usuario al Condominio

| Propiedad        | Valor                                                       |
| ---------------- | ----------------------------------------------------------- |
| **Método**       | `POST`                                                      |
| **Ruta**         | `/api/Condominio/{id}/usuarios`                             |
| **Descripción**  | Asigna un usuario existente del Master al condominio.       |
| **Parámetros**   | `id` (int): ID del condominio.                              |
| **Body**         | `{ "userId": 1 }` (JSON).                                   |
| **Validaciones** | Usuario debe existir en Master. No puede estar ya asignado. |
| **Respuesta**    | `{ "message": "Usuario asignado correctamente" }`.          |

### 3. Eliminar Usuario del Condominio

| Propiedad       | Valor                                                          |
| --------------- | -------------------------------------------------------------- |
| **Método**      | `DELETE`                                                       |
| **Ruta**        | `/api/Condominio/{id}/usuarios/{userId}`                       |
| **Descripción** | Elimina la asignación de un usuario del condominio.            |
| **Parámetros**  | `id` (int): ID del condominio. `userId` (int): ID del usuario. |
| **Respuesta**   | `204 No Content`.                                              |

---

## Servicios

**Ubicación:** `src/Consulcon.Application/Services/Inmuebles/CondominioService.cs`

- `GetUsersAsync(int condominioId)`: Lista usuarios del condominio.
- `AddUserAsync(int condominioId, AddUserToCondominioDto dto)`: Asigna usuario.
- `RemoveUserAsync(int condominioId, int userId)`: Elimina asignación.

---

## Postman Collection

**Archivo:** `docs/99 - Otros/02 - Postman/postman_collection.json`  
**Carpeta:** `Entities > Condominio`

### Request 1: Get Users of Condominio

| Campo       | Valor                                                  |
| ----------- | ------------------------------------------------------ |
| **Nombre**  | Get Users of Condominio                                |
| **Método**  | `GET`                                                  |
| **URL**     | `{{baseUrl}}/api/Condominio/{{condominioId}}/usuarios` |
| **Headers** | `Authorization: Bearer {{authToken}}`                  |

### Request 2: Add User to Condominio

| Campo           | Valor                                                                   |
| --------------- | ----------------------------------------------------------------------- |
| **Nombre**      | Add User to Condominio                                                  |
| **Método**      | `POST`                                                                  |
| **URL**         | `{{baseUrl}}/api/Condominio/{{condominioId}}/usuarios`                  |
| **Headers**     | `Authorization: Bearer {{authToken}}`, `Content-Type: application/json` |
| **Body (JSON)** | Ver ejemplo abajo.                                                      |

**Ejemplo Body:**

```json
{
  "userId": 1
}
```

### Request 3: Remove User from Condominio

| Campo       | Valor                                                    |
| ----------- | -------------------------------------------------------- |
| **Nombre**  | Remove User from Condominio                              |
| **Método**  | `DELETE`                                                 |
| **URL**     | `{{baseUrl}}/api/Condominio/{{condominioId}}/usuarios/1` |
| **Headers** | `Authorization: Bearer {{authToken}}`                    |
