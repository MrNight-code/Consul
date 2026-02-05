# Credenciales de Usuarios

Este documento almacena las credenciales de usuarios, especificando aquellas que no están hasheadas (texto plano) para evitar perder el acceso.

## Usuarios Globales (Master DB)

| Usuario | Contraseña (Real) | Tipo              | Notas                                                                      |
| :------ | :---------------- | :---------------- | :------------------------------------------------------------------------- |
| `admin` | `admin123`        | Hasheada (BCrypt) | Usuario Super Admin por defecto. Credencial inicial creada por el sistema. |

## Usuarios de Tenats (Condominios)

_Actualmente no se han identificado usuarios precargados con contraseñas en texto plano en los scripts de migración. Si se añaden o descubren, regístrelos aquí._

| Usuario     | Contraseña (Real) | Tenant  | Notas   |
| :---------- | :---------------- | :------ | :------ |
| `jgcolodro` | `200218727c`      | Bosques | Migrado |
| `estela`    | `1234567`         | Bosques | Migrado |
| `zarzuri`   | `6205863`         | Bosques | Migrado |
| `uyuni`     | `7227291`         | Bosques | Migrado |
| `maria`     | `68912227`        | Bosques | Migrado |
| `cardenas`  | `7794483`         | Bosques | Migrado |
| `MARIELA`   | `ANCAROSSO`       | Bosques | Migrado |
| `veronica`  | `teteynuria`      | Bosques | Migrado |
| `FLOR`      | `13174403`        | Bosques | Migrado |
| `BETO`      | `14127734`        | Bosques | Migrado |
|             |                   |         |         |

> **Nota Técnica:**
> El sistema ha sido configurado para permitir autenticación híbrida:
>
> 1. Primero intenta verificar el password como Hash BCrypt.
> 2. Si falla o no es un hash válido, compara el password en texto plano directamente con lo almacenado en base de datos.
