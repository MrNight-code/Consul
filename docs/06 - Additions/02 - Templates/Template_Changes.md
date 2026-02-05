# [Number]. [Feature Name] - Cambios

**Sprint:** [Sprint Number]
**Tipo:** Change
**Fecha:** [DD/MM/YYYY]
**Módulo:** [Module Name]

## Descripción

[Description of the change, refactoring, or modification].

## Cambios en Infraestructura

### 1. `ConsulconDbContext`

Ubicación: `src/Consulcon.Infrastructure/Persistence/ConsulconDbContext.cs`

- **Modificación**: [Description of what was added, e.g., new DbSets].
  ```csharp
  // Code example
  public virtual DbSet<Domain.Entities.[Folder].[Entity]> [DbSetName] { get; set; } = null!;
  ```

### 2. Dependency Injection

Ubicación: `src/Consulcon.Infrastructure/DependencyInjection.cs`

- **Modificación**: [Description of new service registration].
  ```csharp
  services.AddScoped<I[Name]Service, Services.[Name]Service>();
  ```

## Cambios en Lógica de Negocio

- **Servicio Afectado**: `[ServiceName]`
- **Nueva Funcionalidad**: [Describe what changed in the logic].
- **Flujo de Datos**: [Explain any changes in how data moves or is stored, e.g., switching from Tenant to Master DB].

## Impacto

- [List potential impacts on other parts of the system].
- [Mention any breaking changes or required migrations].
