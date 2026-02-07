# 08. Feature: Utilidad de Gestión de Usuarios en Condominio

**Sprint:** 03
**Tipo:** Feature
**Fecha:** 06/02/2026
**Módulo:** Inmuebles / Seguridad

## Descripción

Se implementó un nuevo endpoint para permitir la asignación de usuarios existentes a un Condominio. Esto permite que el administrador agregue colaboradores o residentes al condominio recién creado.

## Endpoint

`POST /api/Condominio/{id}/usuarios`

### Request Body

```json
{
  "email": "usuario@ejemplo.com", // O username
  "rolInicial": "Residente" // Opcional, default "Usuario"
}
```

### Respuesta Exitosa (200 OK)

```json
{
  "message": "Usuario asignado correctamente"
}
```

## Cambios Realizados

### 1. DTOs

- **Nuevo Archivo:** `src/Consulcon.Application/DTOs/Inmuebles/AddUserToCondominioDto.cs`
- Define la estructura de entrada (`Email`, `RolInicial`).

### 2. Lógica de Negocio (Servicio)

- **Archivo:** `CondominioService.cs`
- **Método:** `AddUserAsync(int condominioId, AddUserToCondominioDto dto)`
- **Validaciones:**
  - El condominio debe existir.
  - El usuario master debe existir (búsqueda por Email o Username).
  - No debe existir ya una relación entre ese usuario y el condominio.
- **Acción:** Crea un registro en `UsuarioCondominio` (Master DB).

### 3. API (Controlador)

- **Archivo:** `CondominioController.cs`
- **Método:** `AddUser`
- Expone la funcionalidad vía HTTP POST.

## Verificación

- **Compilación:** Verificada.
- **Flujo:** Se puede probar enviando un POST con un email válido de un usuario existente en `UsuariosMaster`.

## Postman Collection

**Archivo:** `docs/99 - Otros/02-postman/postman_collection.json`  
**Carpeta:** `Entities > Condominio`

### Request 1: Agregar Usuario a Condominio

| Campo                  | Valor                                                  |
| ---------------------- | ------------------------------------------------------ |
| **Nombre**             | Add User to Condominio                                 |
| **Método**             | `POST`                                                 |
| **URL**                | `{{baseUrl}}/api/Condominio/{{condominioId}}/usuarios` |
| **Headers**            | `Authorization: Bearer {{authToken}}`                  |
| **Parámetros de Ruta** | `condominioId`: ID del condominio                      |
| **Body**               | JSON con `email` y `rolInicial`                        |
