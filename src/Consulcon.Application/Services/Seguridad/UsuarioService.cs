using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;
using Consulcon.Domain.Entities.Seguridad;
using Consulcon.Domain.Interfaces;

namespace Consulcon.Application.Services.Seguridad;

public class UsuarioService : IUsuarioService
{
    private readonly IRepository<Usuario> _repository;

    public UsuarioService(IRepository<Usuario> repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync(includeProperties: "IdPersonaNavigation");
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<UserDto>> GetByIdAsync(int id)
    {
        var entities = await _repository.FindAsync(u => u.IdUsuario == id, includeProperties: "IdPersonaNavigation");
        var entity = entities.FirstOrDefault();
        
        if (entity == null) return Result.Fail<UserDto>("Usuario no encontrado");
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<UserDto>> CreateAsync(CreateUserDto dto)
    {
        // Hash Password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var entity = new Usuario
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            IdPersona = dto.IdPersona,
            IdRolPrincipal = dto.IdRolPrincipal,
            EstaHabilitado = true,
            FechaCreacion = DateTime.Now
        };

        await _repository.AddAsync(entity);
        
        // Re-fetch to get navigation if needed, or just map manual
        return Result.Ok(new UserDto
        {
            Id = entity.IdUsuario,
            Username = entity.Username,
            FullName = "Pending Load", // Optimistic return or simple
            RoleId = entity.IdRolPrincipal,
            Token = ""
        });
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Usuario no encontrado");

        await _repository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    private static UserDto MapToDto(Usuario entity)
    {
        return new UserDto
        {
            Id = entity.IdUsuario,
            Username = entity.Username,
            FullName = entity.IdPersonaNavigation?.NombreCompleto ?? "Unknown",
            RoleId = entity.IdRolPrincipal,
            Token = ""
        };
    }
}
