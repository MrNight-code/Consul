using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Common;
using Consulcon.Domain.Entities.General;
using Microsoft.EntityFrameworkCore;

namespace Consulcon.Infrastructure.Persistence.Repositories;

// Repositorio especializado para Provider con operaciones de paginación y búsqueda
public class ProviderRepository(ConsulconDbContext context) : EfRepository<Proveedor>(context), IProviderRepository
{
    /// <inheritdoc />
    public async Task<PagedResult<Proveedor>> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Proveedors.AsQueryable();

        // Filtro de activos (soft delete)
        if (activeOnly)
        {
            query = query.Where(p => p.Activo == true);
        }

        // Búsqueda por término (LIKE en RazonSocial o Nit)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                EF.Functions.Like(p.RazonSocial, $"%{term}%") ||
                EF.Functions.Like(p.Nit ?? "", $"%{term}%"));
        }

        // Conteo total antes de paginar
        var totalCount = await query.CountAsync(cancellationToken);

        // Paginación
        var items = await query
            .OrderBy(p => p.RazonSocial)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Proveedor>(items, page, pageSize, totalCount);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
    {
        return await _context.Proveedors
            .AnyAsync(p => p.Nit == taxId && p.Activo == true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByTaxIdAsync(string taxId, int excludeId, CancellationToken cancellationToken = default)
    {
        return await _context.Proveedors
            .AnyAsync(p => p.Nit == taxId && p.Activo == true && p.IdProveedor != excludeId, cancellationToken);
    }
}
