using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Entities.Master;
using Consulcon.Application.Interfaces;
using System.Security.Cryptography;

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

        var dtos = new List<CondominioDto>();
        foreach (var uc in userCondos)
        {
            // Try to get extended data from tenant DB (using TenantId which has sanitized name)
            var dbName = $"db_condominio_{uc.Condominio.TenantId}";
            var tenantData = await _tenantDatabaseService.GetCondominioAsync(dbName);

            if (tenantData != null)
            {
                // Use tenant data (has Direccion, Logo, etc.) but preserve Master Id
                tenantData.IdCondominio = uc.Condominio.Id;
                dtos.Add(tenantData);
            }
            else
            {
                // Fallback to basic data from Master
                dtos.Add(MapToDto(uc.Condominio));
            }
        }
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
        // 1. Create CondominioMaster (basic info only - extended fields stored in Tenant DB)
        var masterEntity = new CondominioMaster
        {
            Nombre = dto.Nombre,
            TenantId = "pending", // Will be updated with sanitized name
            FechaRegistro = DateTime.UtcNow,
            ConnectionString = "" // Will be updated after processing
        };

        await _condominioRepository.AddAsync(masterEntity);

        // Fetch Admin User to get the username
        var adminUser = await _usuarioRepository.GetByIdAsync(userId);
        var adminName = adminUser?.Username ?? "Administrador";

        // Generate DB name from Nombre: lowercase, spaces to underscores, remove special chars
        var sanitizedName = SanitizeDatabaseName(dto.Nombre);
        var dbName = $"db_condominio_{sanitizedName}";
        
        // Build connection string using environment variables (same pattern as DependencyInjection)
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
        
        masterEntity.TenantId = sanitizedName; // Use sanitized name as TenantId
        masterEntity.ConnectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};";
        
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

            // 4. Populate the Tenant Database with the initial Condominio record
            // We use the service to avoid direct dependency on Infrastructure Context
            var initialCondominioData = dto;
            initialCondominioData.IdCondominio = masterEntity.Id;
            initialCondominioData.AdminNombre = adminName;

            await _tenantDatabaseService.InitializeCondominioAsync(dbName, initialCondominioData);

        }
        catch (Exception ex)
        {
             return Result.Fail<CondominioDto>($"Condominio creado, pero falló la configuración de la BD: {ex.Message}");
        }

        return Result.Ok(MapToDto(masterEntity, dto));
    }

    public async Task<Result<CondominioDto>> UpdateAsync(int id, CondominioDto dto)
    {
        var entity = await _condominioRepository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<CondominioDto>("Condominio no encontrado");

        entity.Nombre = dto.Nombre;
        // Note: Direccion, Logo, etc. are stored in Tenant DB, not Master
        
        await _condominioRepository.UpdateAsync(entity);
        
        // TODO: Update tenant DB if needed
        
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _condominioRepository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Condominio no encontrado");

        await _condominioRepository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    private static CondominioDto MapToDto(CondominioMaster entity, CondominioDto? inputDto = null)
    {
        // Master entity only has basic info (Id, TenantId, Nombre)
        // Extended fields (Direccion, Logo, etc.) come from inputDto or Tenant DB
        return new CondominioDto
        {
            IdCondominio = entity.Id,
            Nombre = entity.Nombre,
            Direccion = inputDto?.Direccion,
            SuperficieTotalM2 = inputDto?.SuperficieTotalM2,
            IdAdminPersona = inputDto?.IdAdminPersona ?? 0,
            AdminNombre = inputDto?.AdminNombre,
            ConfigDiaCobro = inputDto?.ConfigDiaCobro,
            Logo = inputDto?.Logo
        };
    }

    /// <summary>
    /// Sanitizes a condominio name to be valid as a database name.
    /// Converts to lowercase, replaces spaces with underscores, removes special characters.
    /// </summary>
    private static string SanitizeDatabaseName(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "unnamed";
        
        // Normalize: lowercase, replace spaces with underscores
        var sanitized = nombre.ToLowerInvariant()
            .Replace(" ", "_")
            .Replace("-", "_");
        
        // Remove any characters that aren't alphanumeric or underscore
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"[^a-z0-9_]", "");
        
        // Remove consecutive underscores
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"_+", "_");
        
        // Trim underscores from start/end
        sanitized = sanitized.Trim('_');
        
        // Ensure it's not empty and not too long (MySQL limit is 64 chars)
        if (string.IsNullOrEmpty(sanitized))
            return "unnamed";
        
        return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
    }

    public async Task<Result<bool>> AddUserAsync(int condominioId, AddUserToCondominioDto dto)
    {
        // 1. Validate Condominio exists
        var condominio = await _condominioRepository.GetByIdAsync(condominioId);
        if (condominio == null) return Result.Fail<bool>("Condominio no encontrado");

        // 2. Validate User exists in Master
        var user = await _usuarioRepository.GetByIdAsync(dto.UserId);
        if (user == null) return Result.Fail<bool>("Usuario no encontrado");

        // 3. Check if already linked
        var existingLink = await _usuarioCondominioRepository.FindAsync(uc => uc.CondominioId == condominioId && uc.UsuarioId == dto.UserId);
        if (existingLink.Any()) return Result.Fail<bool>("El usuario ya está asignado a este condominio");

        // 4. Create Link
        var newLink = new UsuarioCondominio
        {
            CondominioId = condominioId,
            UsuarioId = dto.UserId,
            RolInicial = "Usuario"
        };

        await _usuarioCondominioRepository.AddAsync(newLink);

        return Result.Ok(true);
    }

    public async Task<Result<IEnumerable<CondominioUserDto>>> GetUsersAsync(int condominioId)
    {
        // Validate condominio exists
        var condominio = await _condominioRepository.GetByIdAsync(condominioId);
        if (condominio == null) return Result.Fail<IEnumerable<CondominioUserDto>>("Condominio no encontrado");

        // Get all user links for this condominio
        var links = await _usuarioCondominioRepository.FindAsync(
            uc => uc.CondominioId == condominioId,
            includeProperties: "Usuario"
        );

        var users = links.Select(link => new CondominioUserDto
        {
            UserId = link.UsuarioId,
            Username = link.Usuario.Username,
            FullName = link.Usuario.Email, // Note: UsuarioMaster doesn't have FullName, using Email
            Email = link.Usuario.Email,
            RolInicial = link.RolInicial ?? "Usuario"
        });

        return Result.Ok(users);
    }

    public async Task<Result<bool>> RemoveUserAsync(int condominioId, int userId)
    {
        // Find the link
        var links = await _usuarioCondominioRepository.FindAsync(
            uc => uc.CondominioId == condominioId && uc.UsuarioId == userId
        );
        var link = links.FirstOrDefault();

        if (link == null) return Result.Fail<bool>("El usuario no está asignado a este condominio");

        await _usuarioCondominioRepository.DeleteAsync(link);
        return Result.Ok(true);
    }
}
