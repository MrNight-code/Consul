using Consulcon.Application.Interfaces.Seguridad;
using Consulcon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Consulcon.Infrastructure.Services;

public class MasterIdentityService(ConsulconDbContext context) : IMasterIdentityService
{
    private readonly ConsulconDbContext _context = context;

    public async Task<(int? UserId, string? Username, string? Email, bool? EsSuperAdmin, List<TenantDto>? Tenants)> ValidateUserAsync(string username, string password)
    {
        var user = await _context.UsuariosMaster
            .Include(u => u.Condominios)
            .ThenInclude(uc => uc.Condominio)
            .FirstOrDefaultAsync(u => u.Username == username);


        if (user == null)
        {
            return (null, null, null, null, null);
        }

        bool isValid = false;
        try 
        {
            isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        catch 
        {
            // If Verify throws (e.g. invalid salt/format), we assume it might be plain text
            isValid = false;
        }

        if (!isValid && user.PasswordHash == password)
        {
            isValid = true;
        }

        if (!isValid)
        {
            return (null, null, null, null, null);
        }

        var tenants = user.Condominios.Select(uc => new TenantDto
        {
            Id = uc.Condominio.Id,
            TenantId = uc.Condominio.TenantId,
            Nombre = uc.Condominio.Nombre
        }).ToList();

        return (user.Id, user.Username, user.Email, user.EsSuperAdmin, tenants);
    }
}
