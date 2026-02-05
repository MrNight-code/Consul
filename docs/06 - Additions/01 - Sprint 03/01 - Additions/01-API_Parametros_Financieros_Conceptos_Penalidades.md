# 01. API de Parámetros Financieros: Conceptos y Penalidades

**Sprint:** 03
**Tipo:** Addition
**Fecha:** 26/01/2026

## Visión General

Esta tarea implementa la gestión de **Conceptos de Cobro** y **Configuración Financiera** (Intereses y Mora). Su función principal es permitir al administrador definir _qué_ se cobra (Conceptos) y _cómo_ se penalizan los retrasos (Configuración) para cada condominio de manera independiente.

## ¿Qué entidades usa la DB?

Estas entidades residen en la base de datos de cada **Inquilino (Tenant)** (ej: `db_condominio_1`), no en la base maestra. Esto asegura que cada condominio tenga sus propias reglas y conceptos.

### 1. `ChargeConcept` (Concepto de Cobro)

Ubicación: `src/Consulcon.Domain/Entities/Financiero/ChargeConcept.cs`

- **Propósito**: Define los tipos de cargos que pueden generarse en una expensa.
- **Ejemplos**: "Expensas Ordinarias", "Multa por Ruidos Molestos", "Fondo de Reserva".
- **Propiedades Clave**:
  - `Id`: Identificador único.
  - `CondominiumId`: Vincula el concepto al condominio específico (redundancia útil para validación).
  - `IsRecurrent`: Si es `true`, el motor de expensas lo generará automáticamente cada mes.

### 2. `FinancialConfig` (Configuración Financiera)

Ubicación: `src/Consulcon.Domain/Entities/Financiero/FinancialConfig.cs`

- **Propósito**: Almacena las reglas globales de cobranza para el condominio. Es un Singleton lógico (1 registro por condominio).
- **Propiedades Clave**:
  - `MonthlyInterestRate`: El porcentaje de interés que se aplica a las deudas vencidas.
  - `GraceDays`: Número de días después del vencimiento antes de que se empiecen a calcular intereses.

## ¿Cómo funciona todo esto?

1.  **Configuración Inicial**: El administrador usa los endpoints para crear conceptos (`POST .../concepts`) y definir las reglas de juego (`PUT .../penalties`).
2.  **Persistencia**: Los datos se guardan en las tablas `ChargeConcepts` y `FinancialConfigs` dentro de `ConsulconDbContext`.
3.  **Uso Futuro**: Cuando se corra el proceso de "Generación de Expensas" (futura tarea), el sistema:
    - Consultará `ChargeConcepts` para saber qué ítems agregar a las boletas.
    - Consultará `FinancialConfigs` para calcular recargos a los morosos.

## Nuevos DTOs

Ubicación: `src/Consulcon.Application/DTOs/Financiero/`

- `ChargeConceptDto`: Para lectura.
- `CreateChargeConceptDto`: Para creación.
- `UpdateChargeConceptDto`: Para actualización.
- `FinancialConfigDto`: Para lectura.
- `UpdateFinancialConfigDto`: Para actualización.

## Nuevo Controller

Ubicación: `src/Consulcon.API/Controllers/Financiero/FinancialConfigController.cs`

- Ruta Base: `api/FinancialConfig`

### Endpoints

#### Penalidades / Configuración

- `GET /api/FinancialConfig/penalties/{condominiumId}`: Obtiene la configuración de intereses y mora.
- `PUT /api/FinancialConfig/penalties/{condominiumId}`: Actualiza la configuración.

#### Conceptos de Cobro

- `GET /api/FinancialConfig/concepts/{condominiumId}`: Listar conceptos activos.
- `POST /api/FinancialConfig/concepts/{condominiumId}`: Crear nuevo concepto.
- `PUT /api/FinancialConfig/concepts/{id}`: Actualizar concepto.
- `DELETE /api/FinancialConfig/concepts/{id}`: Eliminar (Soft Delete) concepto.

## Servicios

Ubicación: `src/Consulcon.Infrastructure/Services/FinancialConfigService.cs`

- Implementa `IFinancialConfigService`.
- **Funcionalidad**: Valida que los IDs correspondan al condominio y ejecuta las operaciones CRUD.
- **Interacción DB**: Usa `IRepository<ChargeConcept>` y `IRepository<FinancialConfig>` inyectados vía DI.

## Preguntas Frecuentes (FAQ Técnico)

### 1. ¿Se crearon nuevas tablas en la DB?

**Sí.** Para esta funcionalidad específicamente, se añaden las tablas `ChargeConcepts` y `FinancialConfigs` en la base de datos de cada inquilino. Estas tablas son donde se persisten las entidades creadas.

### 2. ¿Es realmente óptimo crear estas tablas desde el código (Code-First)?

**Sí, es una práctica estándar y recomendada.**

- **Consistencia**: Asegura que la estructura de la base de datos coincida exactamente con lo que el código espera, evitando errores en tiempo de ejecución.
- **Mantenibilidad**: Es más fácil rastrear cambios en el modelo de datos (git) que cambios manuales en scripts SQL.
- **Automatización**: Permite despliegues automatizados sin intervención manual en la DB.

### 3. ¿Las tablas se crean automáticamente cuando se llama al endpoint?

**Depende del entorno:**

- **Nuevos Condominios**: Sí. Cuando se crea un condominio nuevo (`POST /api/Condominio`), el sistema ejecuta `EnsureCreated()`, lo que crea todas estas tablas automáticamente.
- **Condominios Existentes**: No al momento de llamar al endpoint de Finanzas. Se requiere haber corrido una **Migración** previa para actualizar la estructura de la base de datos existente. Si intentas usar los endpoints en una DB vieja sin migrar, fallará.
