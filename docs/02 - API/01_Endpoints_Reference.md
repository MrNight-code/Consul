# Referencia de API Endpoints (Consulcon)

Documentación actualizada y sincronizada con la colección de Postman y el Backend (Swagger).
**Base URL:** `http://localhost:3010` (Local Dev) | `http://localhost:5000` (Docker) | `https://tu-dominio.railway.app` (Nube)

## 🌎 Autenticación Centralizada y Arquitectura Multi-Tenant

El sistema implementa un modelo de autenticación híbrido que permite a los usuarios iniciar sesión globalmente y luego operar en el contexto de un tenant específico.

### Flujo de Autenticación

#### 1. Login Global (Discovery)

El usuario inicia sesión sin especificar un tenant. El sistema valida las credenciales contra la **Base de Datos Maestra**.

- **Endpoint**: `POST /api/auth/login`
- **Headers**: Ninguno (o sin `X-Tenant-Id`)
- **Respuesta (200 OK)**: Retorna el token JWT y la lista de condominios disponibles.

#### 2. Selección de Tenant

Una vez obtenido el token y la lista de tenants, el cliente debe seleccionar en qué condominio desea operar.

#### 3. Operaciones por Tenant

Para cualquier petición que requiera contexto de datos, se debe incluir el header `X-Tenant-Id`.

### Headers Requeridos

| Header          | Ejemplo              | ¿Obligatorio?          | Descripción                                                      |
| :-------------- | :------------------- | :--------------------- | :--------------------------------------------------------------- |
| `Authorization` | `Bearer eyJhbGci...` | **SÍ** (excepto login) | Token JWT obtenido del login global.                             |
| `X-Tenant-Id`   | `foret` o `bosques`  | **SÍ**                 | Define a qué base de datos conectarse (`db_condominio_{valor}`). |

---

## 📋 Resumen de Endpoints

| #   | Módulo       | Nombre                        | Endpoint                                    | Descripción                                             |
| --- | ------------ | ----------------------------- | ------------------------------------------- | ------------------------------------------------------- |
| 1   | Seguridad    | Login                         | `POST /api/auth/login`                      | Obtiene el token JWT para autenticación.                |
| 2   | Dashboard    | Get Contadores                | `GET /api/Dashboard/{condominioId}`         | Obtiene contadores agregados del dashboard.             |
| 3   | Dashboard    | Refrescar Contadores          | `POST /api/Dashboard/{id}/refrescar`        | Refresca/recalcula los contadores del dashboard.        |
| 4   | Inmobiliario | Get All Condominios           | `GET /api/Condominio`                       | Lista todos los condominios de la DB actual.            |
| 5   | Inmobiliario | Create Condominio             | `POST /api/Condominio`                      | Registra datos de un condominio.                        |
| 6   | Inmobiliario | Get All Propiedades           | `GET /api/Propiedad`                        | Lista todas las propiedades de la DB actual.            |
| 7   | Inmobiliario | Get Propiedades by Condominio | `GET /api/Propiedad/condominio/{id}`        | Filtra propiedades por ID condominio.                   |
| 8   | Inmobiliario | Create Propiedad              | `POST /api/Propiedad`                       | Crea una unidad funcional (Depto/Casa).                 |
| 9   | Propiedad    | Assign Owner                  | `POST /api/Ownership/assign-owner`          | Asigna propietario (cierra anterior, abre nuevo).       |
| 10  | Propiedad    | Get History                   | `GET /api/Ownership/history/{id}`           | Historial de propietarios de una unidad.                |
| 11  | Contractual  | Get All Contratos             | `GET /api/Contrato`                         | Lista contratos de alquiler/venta.                      |
| 12  | Contractual  | Create Contrato               | `POST /api/Contrato`                        | Crea un contrato nuevo vinculando propiedad y personas. |
| 13  | Contractual  | Add Participante              | `POST /api/Contrato/{id}/participante`      | Agrega un garante o inquilino extra a un contrato.      |
| 14  | Servicios    | Get All Servicios             | `GET /api/CatalogoServicio`                 | Lista tipos de cobros disponibles (Agua, Expensa).      |
| 15  | Servicios    | Create Servicio               | `POST /api/CatalogoServicio`                | Crea un nuevo concepto de cobro.                        |
| 16  | Financiero   | Get Penalties                 | `GET /api/FinancialConfig/penalties/{id}`   | Configuración de intereses/moras.                       |
| 17  | Financiero   | Update Penalties              | `PUT /api/FinancialConfig/penalties/{id}`   | Actualiza intereses y día de corte.                     |
| 18  | Financiero   | Get Concepts                  | `GET /api/FinancialConfig/concepts/{id}`    | Conceptos de cobro (Expensas, Reservas).                |
| 19  | Financiero   | Create Concept                | `POST /api/FinancialConfig/concepts/{id}`   | Crea concepto financiero.                               |
| 20  | Deudas       | Generar Deuda                 | `POST /api/deuda/generar`                   | Genera deuda (expensa) manual o automática.             |
| 21  | Deudas       | Get Pending Debts             | `GET /api/deuda/pendiente`                  | Consulta deudas impagas.                                |
| 22  | Deudas       | Get by Contrato               | `GET /api/deuda/contrato/{id}`              | Historial de deudas de un contrato.                     |
| 23  | Pagos        | Registrar Pago                | `POST /api/pago`                            | Registra el pago de una deuda.                          |
| 24  | Cobranzas    | Generar Recibo                | `POST /api/cobranzas/{id}/generar-recibo`   | Genera y congela el recibo PDF.                         |
| 25  | Cobranzas    | Listar Recibos                | `GET /api/cobranzas/recibos`                | Lista recibos generados con filtros.                    |
| 26  | Tesorería    | Get Bancos                    | `GET /api/tesoreria/bancos`                 | Lista cuentas bancarias del condominio.                 |
| 27  | Tesorería    | Create Banco                  | `POST /api/tesoreria/bancos`                | Registra una nueva cuenta bancaria.                     |
| 28  | Tesorería    | Registrar Egreso              | `POST /api/tesoreria/egresos`               | Registra un gasto/compra.                               |
| 29  | Tesorería    | Get Formas Pago               | `GET /api/tesoreria/formaspago`             | Lista formas de pago (Efectivo, Transferencia).         |
| 30  | Contabilidad | Get Plan Cuentas              | `GET /api/contabilidad/plancuentas`         | Obtiene el árbol de cuentas contables.                  |
| 31  | Contabilidad | Registrar Asiento             | `POST /api/contabilidad/asientos`           | Crea un asiento contable manual.                        |
| 32  | Contabilidad | Get Autorizaciones            | `GET /api/contabilidad/autorizaciones`      | Niveles de autorización de gasto.                       |
| 33  | Eventos      | Get Recursos                  | `GET /api/reserva/recursos/condominio/{id}` | Lista áreas comunes (Salones, Parrillas).               |
| 34  | Eventos      | Create Reserva                | `POST /api/reserva`                         | Reserva un área común para una fecha.                   |
| 35  | Comunicación | Get Comunicados               | `GET /api/comunicacion/condominio/{id}`     | Lista noticias/avisos del condominio.                   |
| 36  | Comunicación | Create Comunicado             | `POST /api/comunicacion`                    | Publica un nuevo aviso.                                 |
| 37  | Personas     | Get All Personas              | `GET /api/persona`                          | CRUD Personas (Residentes, Propietarios).               |
| 38  | Personas     | Create Persona                | `POST /api/persona`                         | Crea nueva persona física/jurídica.                     |
| 39  | Usuarios     | Get All Usuarios              | `GET /api/usuario`                          | Listado de usuarios del sistema.                        |
| 40  | Config       | Cuentas (Destino)             | `GET /api/accounts`                         | Configuración de cuentas receptoras (CRUD).             |
| 41  | Admin        | Migrar Tenant                 | `POST /api/maintenance/migrate/{id}`        | Ejecuta migraciones de DB manuales.                     |
| 42  | Usuarios     | Get Roles                     | `GET /api/rol`                              | Listado de roles del sistema.                           |

---

## 🔐 01. Seguridad (Auth)

### Login

- **Endpoint**: `POST /api/auth/login`
- **Desc**: Autenticación para obtener Token JWT. Requiere `X-Tenant-Id` para contexto específico, o sin él para Discovery global.

---

## 📊 02. Dashboard

### Get Contadores

- **Endpoint**: `GET /api/Dashboard/{condominioId}`
- **Desc**: Totales, morosidad, recaudación mes actual.

### Refrescar

- **Endpoint**: `POST /api/Dashboard/{condominioId}/refrescar`
- **Desc**: Fuerza recalculo de métricas.

---

## 🏢 03. Inmobiliario (Condominio, Propiedad)

### Condominio

- **Get All**: `GET /api/Condominio`
- **Create**: `POST /api/Condominio`

### Propiedad

- **Get All**: `GET /api/Propiedad`
- **Get by Condominio**: `GET /api/Propiedad/condominio/{id}`
- **Create**: `POST /api/Propiedad`

### Ownership (Asignación)

- **Assign Owner**: `POST /api/Ownership/assign-owner`
  - Cierra titularidad anterior y crea nueva transacción.
- **Get History**: `GET /api/Ownership/history/{propiedadId}`

---

## 📜 04. Contractual (Contratos)

- **Get All**: `GET /api/Contrato`
- **Create**: `POST /api/Contrato`
- **Add Participante**: `POST /api/Contrato/{id}/participante` (Para agregar Garantes/Ocupantes extras).

---

## 💡 05. Servicios y Configuración Financiera

### Catálogo de Servicios

- **Get All**: `GET /api/CatalogoServicio`
- **Create**: `POST /api/CatalogoServicio`

### Configuración Financiera (Intereses/Multas)

- **Get Config**: `GET /api/FinancialConfig/penalties/{condominiumId}`
- **Update Config**: `PUT /api/FinancialConfig/penalties/{condominiumId}`
  - Define interés moratorio diario/mensual y día de corte.

### Conceptos de Cobro (Financial Concepts)

- **Get Concepts**: `GET /api/FinancialConfig/concepts/{condominiumId}`
- **Create**: `POST /api/FinancialConfig/concepts/{condominiumId}`
- **Update/Delete**: `PUT/DELETE /api/FinancialConfig/concepts/{id}`

---

## 💰 06. Facturación y Cobranzas

### Deudas

- **Generar**: `POST /api/deuda/generar`
- **Pendientes**: `GET /api/deuda/pendiente`
- **Por Contrato**: `GET /api/deuda/contrato/{contratoId}`

### Pagos

- **Registrar Pago**: `POST /api/pago`
  - Body: `{ idDeuda, idFormaPago, montoAbonado, ... }`

### Cobranzas

- **Registrar Cobranza**: `POST /api/cobranzas`
  - Body: `{ unitId, monto, idFormaPago, ... }`
- **Historial de Cobranzas**: `GET /api/cobranzas/{unitId}`
- **Generar Recibo**: `POST /api/cobranzas/{id}/generar-recibo`
  - Genera el PDF del recibo y lo almacena.
- **Listar Recibos**: `GET /api/cobranzas/recibos`
  - Filtros: `FechaDesde`, `FechaHasta`, `PersonaId`.
- **Descargar Recibo**: `GET /api/cobranzas/recibos/{filename}`

---

## 🏦 07. Tesorería

- **Bancos**:
  - `GET /api/tesoreria/bancos`
  - `POST /api/tesoreria/bancos`
- **Formas de Pago**:
  - `GET /api/tesoreria/formaspago`
  - `POST /api/tesoreria/formaspago`
- **Egresos**:
  - `GET /api/tesoreria/egresos/condominio/{id}`
  - `POST /api/tesoreria/egresos`

### Cuentas Destino (Configuración)

- **Endpoint Base**: `/api/accounts`
- **Operaciones**: CRUD completo (`GET`, `POST`, `PUT`, `DELETE`).
- **Uso**: Configura las cuentas donde se recibirán los pagos (puede estar relacionado con Tesorería/Bancos).

---

## 📉 08. Contabilidad

- **Plan de Cuentas**: `GET` / `POST` `/api/contabilidad/plancuentas`
- **Asientos**: `GET` / `POST` `/api/contabilidad/asientos`
- **Autorizaciones**: `GET` / `POST` `/api/contabilidad/autorizaciones`

---

## 📅 09. Eventos (Reservas)

- **Recursos**: `GET /api/reserva/recursos/condominio/{id}`
- **Crear Reserva**: `POST /api/reserva`

---

## 📢 10. Comunicación

- **Ver Comunicados**: `GET /api/comunicacion/condominio/{id}`
- **Crear Comunicado**: `POST /api/comunicacion`
- **Borrar**: `DELETE /api/comunicacion/{id}`

---

## 👥 11. Personas y Usuarios

### Personas

- **Base**: `/api/persona`
- **Operaciones**: CRUD completo.

### Usuarios

- **Base**: `/api/usuario`
- **Operaciones**: `GET`, `POST`, `DELETE`.

### Roles

- **Base**: `/api/rol`
- **Operaciones**: `GET`.

---

## 🛠️ 12. Administración (System)

### Mantenimiento

- **Migrar DB Tenant**: `POST /api/maintenance/migrate/{tenantId}`
- **Desc**: Ejecuta manualmente las migraciones de EF Core para un tenant específico. (Solo Admin).
