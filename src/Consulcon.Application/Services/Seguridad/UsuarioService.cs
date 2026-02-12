using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Consulcon.Domain.Entities.Master;
using Consulcon.Domain.Interfaces;

namespace Consulcon.Application.Services.Seguridad;

public class UsuarioService : IUsuarioService
{
    private readonly IRepository<UsuarioMaster> _repository;

    public UsuarioService(IRepository<UsuarioMaster> repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<UserDto>> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        
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

    private static UserDto MapToDto(UsuarioMaster entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            FullName = entity.Email ?? entity.Username, // Use email or username as display name
            RoleId = entity.EsSuperAdmin ? 1 : null, // SuperAdmin = role 1
            Token = ""
        };
    }
}
