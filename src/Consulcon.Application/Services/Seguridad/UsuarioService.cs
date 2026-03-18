using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Consulcon.Domain.Entities.Master;
using Consulcon.Domain.Interfaces;

namespace Consulcon.Application.Services.Seguridad;

public class UsuarioService(IRepository<UsuarioMaster> repository, IRepository<UsuarioCondominio> usuarioCondominioRepository) : IUsuarioService
{
    private readonly IRepository<UsuarioMaster> _repository = repository;
    private readonly IRepository<UsuarioCondominio> _usuarioCondominioRepository = usuarioCondominioRepository;

    public async Task<Result<IEnumerable<UserDto>>> GetAllAsync()
    {
        var entities = await _repository.FindAsync(u => true, includeProperties: "Condominios");
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<UserDto>> GetByIdAsync(int id)
    {
        var entities = await _repository.FindAsync(u => u.Id == id, includeProperties: "Condominios");
        var entity = entities.FirstOrDefault();
        
        if (entity == null) return Result.Fail<UserDto>("Usuario no encontrado");
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserDto dto)
    {
        // Check if username already exists
        var existing = await _repository.FindAsync(u => u.Username == dto.Username);
        if (existing.Any())
        {
            return Result.Fail<UserDto>("El nombre de usuario ya existe");
        }

        // Hash Password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var entity = new UsuarioMaster
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            Email = dto.Email,
            EsSuperAdmin = false,
            FechaCreacion = DateTime.UtcNow
        };

        await _repository.AddAsync(entity);
        
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Usuario no encontrado");

        // Don't allow deleting the last super admin
        if (entity.EsSuperAdmin)
        {
            var superAdmins = await _repository.FindAsync(u => u.EsSuperAdmin);
            if (superAdmins.Count() <= 1)
            {
                return Result.Fail<bool>("No se puede eliminar el último super administrador");
            }
        }

        await _repository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    public async Task<Result<UserDto>> UpdateAsync(int id, UpdateUserDto dto)
    {
        var entities = await _repository.FindAsync(u => u.Id == id, includeProperties: "Condominios");
        var entity = entities.FirstOrDefault();
        
        if (entity == null) return Result.Fail<UserDto>("Usuario no encontrado");

        if (!string.IsNullOrWhiteSpace(dto.Username))
        {
            var existing = await _repository.FindAsync(u => u.Username == dto.Username && u.Id != id);
            if (existing.Any())
            {
                return Result.Fail<UserDto>("El nombre de usuario ya existe");
            }
            entity.Username = dto.Username;
        }

        if (dto.Email != null)
        {
            entity.Email = dto.Email;
        }

        if (dto.IdRolPrincipal.HasValue)
        {
            entity.IdRolPrincipal = dto.IdRolPrincipal.Value;
        }

        if (!string.IsNullOrEmpty(dto.PasswordTemporal))
        {
            // TODO: Implementar lógica de contraseña temporal (forzar cambio en el siguiente inicio de sesión)
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordTemporal);
        }

        if (dto.CondominioIds != null)
        {
            // Remove existing links
            var existingLinks = entity.Condominios.ToList();
            foreach (var link in existingLinks)
            {
                await _usuarioCondominioRepository.DeleteAsync(link);
            }

            // Create new links
            foreach (var condoId in dto.CondominioIds)
            {
                var newLink = new UsuarioCondominio
                {
                    UsuarioId = id,
                    CondominioId = condoId,
                    IdRol = dto.IdRolPrincipal ?? 3 // Default a operador si no se especifica
                };
                await _usuarioCondominioRepository.AddAsync(newLink);
            }
            
            // Reload entity to get updated condominios
            entities = await _repository.FindAsync(u => u.Id == id, includeProperties: "Condominios");
            entity = entities.FirstOrDefault()!;
        }

        await _repository.UpdateAsync(entity);
        return Result.Ok(MapToDto(entity));
    }

    private static UserDto MapToDto(UsuarioMaster entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            RoleId = entity.IdRolPrincipal,
            EsSuperAdmin = entity.EsSuperAdmin,
            CondominioIds = entity.Condominios?.Select(uc => uc.CondominioId).ToList()
        };
    }
}
