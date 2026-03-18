using Consulcon.Application.DTOs.Seguridad;
using Consulcon.Application.Interfaces.Seguridad;

namespace Consulcon.Application.Services.Seguridad;

public class AuthService(
    IMasterIdentityService masterIdentityService,
    IJwtTokenGenerator jwtTokenGenerator,
    IRepository<Usuario> usuarioRepository,
    ICurrentTenantService tenantService) : IAuthService
{
    private readonly IMasterIdentityService _masterIdentityService = masterIdentityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IRepository<Usuario> _usuarioRepository = usuarioRepository;
    private readonly ICurrentTenantService _tenantService = tenantService;

    public async Task<Result<UserDto>> LoginAsync(string username, string password)
    {
        // 1. Check if we are in a Tenant Context
        if (string.IsNullOrEmpty(_tenantService.TenantId))
        {
            // Global Login -> Validation against Master DB
            var (userId, userObjUsername, email, esSuperAdmin, tenants) = await _masterIdentityService.ValidateUserAsync(username, password);
             
            if (userId == null)
            {
               return Result.Fail<UserDto>("Credenciales inválidas (Global).");
            }

            // Return special payload with available tenants
            // For now, we reuse UserDto but maybe we should add "Tenants" property or just return them loosely if the DTO supports it.
            // Since DTO is fixed, let's look at how to pass this info. 
            // The FE expects { data: { token: "..." } }.
            // We can encode the authorized tenants in the Token claims OR return a specific object if we change the return type.
            // But strict signature: Task<Result<UserDto>>.
            
            // Let's create a temporary token that basically says "I am authenticated globally, but need to select a tenant".
            // OR simpler: Return the data and let the frontend decide.
            
            // We need to extend UserDto to support 'Tenants'.
            
            var simpleDto = new UserDto
            {
                Id = userId.Value,
                Username = userObjUsername!,
                Email = email,
                RoleId = esSuperAdmin == true ? 1 : 0,
                EsSuperAdmin = esSuperAdmin ?? false,
                Token = string.Empty,
                CondominioIds = tenants?.Select(t => t.Id).ToList()
            };

            // Generate token for Global User
            simpleDto.Token = _jwtTokenGenerator.GenerateToken(simpleDto);
            
            return Result.Ok(simpleDto);
        }

        // 2. Tenant Login -> Validation against Tenant DB
        // User must exist in local DB (synced)
        var users = await _usuarioRepository.FindAsync(u => u.Username == username, includeProperties: "IdPersonaNavigation");
        var user = users.FirstOrDefault();

        // Password Verification
        // Note: In a synced environment, we might want to trust the Master password hash OR correct local hash.
        // If we synced the hash, local check works.


        bool passwordNeedsRehash = false;

        if (user == null)
        {
             return Result.Fail<UserDto>("Credenciales inválidas (Tenant).");
        }

        bool isValid = false;
        try 
        {
            isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        catch 
        {
            isValid = false;
        }

        if (!isValid && user.PasswordHash == password)
        {
            isValid = true;
            passwordNeedsRehash = true;
        }

        if (!isValid)
        {
             return Result.Fail<UserDto>("Credenciales inválidas (Tenant).");
        }

        // Automatic Migration: Hash the plain text password
        if (passwordNeedsRehash)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _usuarioRepository.UpdateAsync(user);
        }

        var dto = new UserDto
        {
            Id = user.IdUsuario,
            Username = user.Username
        };
        
        dto.Token = _jwtTokenGenerator.GenerateToken(dto);

        return Result.Ok(dto);
    }
}
