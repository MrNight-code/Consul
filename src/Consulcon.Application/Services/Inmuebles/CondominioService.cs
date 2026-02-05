using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Consulcon.Domain.Entities.Inmuebles; // Keep for mapping if needed or remove if unused. 
// We need Master entities
using Consulcon.Domain.Entities.Master; 
using Consulcon.Application.Interfaces; // For ITenantDatabaseService
using System.Security.Cryptography; // not strictly needed unless generating random things

namespace Consulcon.Application.Services.Inmuebles;

public class CondominioService : ICondominioService
{
    private readonly IRepository<CondominioMaster> _condominioRepository;
    private readonly IRepository<UsuarioMaster> _usuarioRepository;
    private readonly IRepository<UsuarioCondominio> _usuarioCondominioRepository;
    private readonly ITenantDatabaseService _tenantDatabaseService;
    private readonly ITenantMigrationService _tenantMigrationService;

    public CondominioService(
        IRepository<CondominioMaster> condominioRepository,
        IRepository<UsuarioMaster> usuarioRepository,
        IRepository<UsuarioCondominio> usuarioCondominioRepository,
        ITenantDatabaseService tenantDatabaseService,
        ITenantMigrationService tenantMigrationService)
    {
        _condominioRepository = condominioRepository;
        _usuarioRepository = usuarioRepository;
        _usuarioCondominioRepository = usuarioCondominioRepository;
        _tenantDatabaseService = tenantDatabaseService;
        _tenantMigrationService = tenantMigrationService;
    }

    public async Task<Result<IEnumerable<CondominioDto>>> GetAllAsync(int userId)
    {
        var userCondos = await _usuarioCondominioRepository.FindAsync(
            uc => uc.UsuarioId == userId,
            includeProperties: "Condominio"
        );

        var dtos = userCondos.Select(uc => MapToDto(uc.Condominio)).ToList();
        return Result.Ok<IEnumerable<CondominioDto>>(dtos);
    }

    public async Task<Result<CondominioDto>> GetByIdAsync(int id)
    {
        var entity = await _condominioRepository.GetByIdAsync(id);
        
        if (entity == null) return Result.Fail<CondominioDto>("Condominio no encontrado");

        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<CondominioDto>> CreateAsync(CondominioDto dto, int userId)
    {
        // 1. Create CondominioMaster
        var masterEntity = new CondominioMaster
        {
            Nombre = dto.Nombre,
            TenantId = "temp_tenant_id", 
            FechaRegistro = DateTime.UtcNow,
            ConnectionString = "" 
        };

        await _condominioRepository.AddAsync(masterEntity);

        // Update TenantId based on generated ID to ensure uniqueness
        masterEntity.TenantId = $"condominio_{masterEntity.Id}";
        var dbName = $"db_{masterEntity.TenantId}";
        // masterEntity.ConnectionString could be set here if needed, but we rely on naming convention in TenantDatabaseService often.
        
        await _condominioRepository.UpdateAsync(masterEntity);


        // 2. Link to User
        var userLink = new UsuarioCondominio
        {
            UsuarioId = userId,
            CondominioId = masterEntity.Id,
            RolInicial = "Administrador"
        };
        await _usuarioCondominioRepository.AddAsync(userLink);

        // 3. Create Tenant Database & Run Migrations
        try 
        {
            await _tenantDatabaseService.CreateDatabaseAsync(dbName);
            await _tenantDatabaseService.InitializeDatabaseAsync(dbName);
            
            // Apply any extra SQL migrations
            await _tenantMigrationService.MigrateTenantDatabaseAsync(dbName);
        }
        catch (Exception ex)
        {
             return Result.Fail<CondominioDto>($"Condominio creado, pero falló la configuración de la BD: {ex.Message}");
        }

        return Result.Ok(MapToDto(masterEntity));
    }

    public async Task<Result<CondominioDto>> UpdateAsync(int id, CondominioDto dto)
    {
        var entity = await _condominioRepository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<CondominioDto>("Condominio no encontrado");

        entity.Nombre = dto.Nombre;
        
        await _condominioRepository.UpdateAsync(entity);
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _condominioRepository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Condominio no encontrado");

        await _condominioRepository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    private static CondominioDto MapToDto(CondominioMaster entity)
    {
        return new CondominioDto
        {
            Id = entity.Id,
            Codigo = entity.TenantId,
            Nombre = entity.Nombre,
            // Fields not present in CondominioMaster are returned as null/default/empty
            Direccion = null,
            SuperficieTotalM2 = null,
            IdAdminPersona = 0, 
            AdminNombre = null,
            ConfigDiaCobro = null,
            Logo = null
        };
    }
}
