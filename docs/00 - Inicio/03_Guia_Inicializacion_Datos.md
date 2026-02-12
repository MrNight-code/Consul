# Guía de Inicialización y Flujo de Datos

Esta guía detalla el orden correcto de creación de datos para configurar un nuevo Condominio desde cero y evitar errores de dependencias (ej: "La unidad con ID 1 no existe").

## Resumen del Flujo de Datos

1.  **Nivel Master**: Crear Condominio (generación de Base de Datos).
2.  **Nivel Tenant (Infraestructura)**: Crear Bloques/Manzanos -> Crear Propiedades/Unidades.
3.  **Nivel Tenant (Personas)**: Crear Propietarios/Inquilinos.
4.  **Nivel Tenant (Contratos)**: Asignar Personas a Propiedades (Genera Contrato).
5.  **Nivel Tenant (Deudas/Pagos)**: Generar Deudas -> Registrar Cobranzas.

---

## Paso 1: Creación del Condominio (Nivel Master)

**Endpoint:** `POST /api/Condominio`
**Header:** `Authorization: Bearer <token>` (Sin header de Condominio)

Al crear un condominio, el sistema:

1.  Registra el condominio en la base de datos Master.
2.  Crea una **Base de Datos nueva y vacía** para ese condominio (ej: `db_condominio_nuevo`).
3.  Ejecuta las migraciones iniciales (tablas vacías).

> [!WARNING]
> En este punto, la base de datos del tenant existe pero NO TIENE DATOS. No existen manzanas, propiedades, ni personas. Cualquier intento de registrar pagos fallará.

---

## Paso 2: Infraestructura Física

Debes poblar la estructura física del condominio.

### 2.1 Crear Manzanos (Bloques/Torres)

**Endpoint:** `POST /api/Manzano`
**Header:** `X-Condominio-Id: <ID_DEL_CONDOMINIO>`

Ejemplo Body:

```json
{
  "nombre": "Manzano A",
  "descripcion": "Torre Norte"
}
```

### 2.2 Crear Propiedades (Unidades/Departamentos)

**Endpoint:** `POST /api/Propiedad`
**Header:** `X-Condominio-Id: <ID_DEL_CONDOMINIO>`

Requiere el ID del Manzano creado en el paso 2.1.

Ejemplo Body:

```json
{
  "manzanoId": 1,
  "nombreFuncional": "101-A",
  "tipo": "Departamento", // O "Casa", "Lote"
  "superficieM2": 80.5,
  "porcentajeParticipacion": 1.2
}
```

---

## Paso 3: Registro de Personas

Registrar a los futuros dueños o inquilinos.

**Endpoint:** `POST /api/Persona`
**Header:** `X-Condominio-Id: <ID_DEL_CONDOMINIO>`

Ejemplo Body:

```json
{
  "nombre": "Juan",
  "apellido": "Pérez",
  "ci": "1234567",
  "email": "juan@example.com",
  "telefono": "700123456"
}
```

---

## Paso 4: Asignación de Propiedad (Contratos)

Este es el paso crucial que vincula una **Persona** con una **Propiedad**. Sin esto, no se pueden generar deudas ni cobrar expensas.

**Endpoint:** `POST /api/ownership/assign-participant`
**Header:** `X-Condominio-Id: <ID_DEL_CONDOMINIO>`

Ejemplo Body:

```json
{
  "unitId": 1, // ID de la Propiedad (Paso 2.2)
  "personId": 1, // ID de la Persona (Paso 3)
  "role": "Owner", // "Owner" o "Tenant"
  "startDate": "2024-01-01"
}
```

> [!IMPORTANT]
> Al realizar esta asignación, el sistema crea internamente un **Contrato**. Las cobranzas se validan contra este contrato activo.

---

## Paso 5: Gestión de Cobranzas

Ahora que existe una Propiedad con un Dueño (Contrato), puedes registrar pagos.

### 5.1 Registrar Cobranza

**Endpoint:** `POST /api/cobranzas`
**Header:** `X-Condominio-Id: <ID_DEL_CONDOMINIO>`

Requiere el ID de la **Propiedad** (Unidad), no del contrato ni de la persona. El sistema busca automáticamente el contrato activo para esa unidad.

Ejemplo Body:

```json
{
  "unitId": 1, // ID de la Propiedad
  "monto": 500.0,
  "idFormaPago": 1, // Efectivo, Transferencia, etc. (Ver /api/FormaPago)
  "nroReferencia": "REC-001",
  "observaciones": "Expensa Enero",
  "idBancoDestino": 1 // Cuenta bancaria del consorcio (Ver /api/Banco)
}
```

---

## Errores Comunes

### "La unidad con ID X no existe"

- **Causa:** Estás enviando un `unitId` que no ha sido creado en la tabla `propiedad`.
- **Solución:** Verifica haber ejecutado el Paso 2.2 y usa el ID correcto retornado por la creación.

### "No se encontró contrato activo"

- **Causa:** La unidad existe, pero no tiene dueño asignado, o la fecha del pago está fuera de la vigencia del contrato.
- **Solución:** Ejecuta el Paso 4 para asignar un dueño.

### "Table 'db_consulcon_master.propiedad' doesn't exist"

- **Causa:** (Deprecated) Ocurría cuando el Condominio ID no existía y el sistema caía al Master DB.
- **Solución:** Ahora recibirás un error 400 `ERR-TENANT-404` indicando que el condominio no existe. Verifica tu header `X-Condominio-Id`.
