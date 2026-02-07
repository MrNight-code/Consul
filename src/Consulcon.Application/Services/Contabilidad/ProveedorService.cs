using Consulcon.Application.DTOs.Contabilidad;
using Consulcon.Application.Interfaces.Contabilidad;
using Consulcon.Domain.Entities.General;
using Consulcon.Domain.Interfaces;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Services.Contabilidad;

public class ProveedorService : IProveedorService
{
    private readonly IRepository<Proveedor> _repository;
    private readonly IProviderRepository _providerRepository;

    public ProveedorService(IRepository<Proveedor> repository, IProviderRepository providerRepository)
    {
        _repository = repository;
        _providerRepository = providerRepository;
    }

    #region Métodos Legacy (compatibilidad hacia atrás)

    public async Task<Result<IEnumerable<ProveedorDto>>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return Result.Ok(entities.Select(MapToLegacyDto));
    }

    public async Task<Result<ProveedorDto>> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<ProveedorDto>("Proveedor no encontrado");
        return Result.Ok(MapToLegacyDto(entity));
    }

    public async Task<Result<ProveedorDto>> CreateAsync(ProveedorDto dto)
    {
        var entity = new Proveedor
        {
            RazonSocial = dto.RazonSocial,
            Nit = dto.Nit,
            Contacto = dto.Contacto,
            Direccion = dto.Direccion,
            Activo = dto.Activo ?? true
        };

        await _repository.AddAsync(entity);
        return Result.Ok(MapToLegacyDto(entity));
    }

    public async Task<Result<ProveedorDto>> UpdateAsync(int id, ProveedorDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<ProveedorDto>("Proveedor no encontrado");

        entity.RazonSocial = dto.RazonSocial;
        entity.Nit = dto.Nit;
        entity.Contacto = dto.Contacto;
        entity.Direccion = dto.Direccion;
        entity.Activo = dto.Activo ?? true;

        await _repository.UpdateAsync(entity);
        return Result.Ok(MapToLegacyDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Proveedor no encontrado");

        await _repository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    #endregion

    #region Nuevos Métodos API Providers

    /// <inheritdoc />
    public async Task<Result<PagedResult<ProviderDto>>> GetPagedAsync(
        int page = 1,
        int pageSize = 20,
        string? term = null,
        CancellationToken cancellationToken = default)
    {
        // Validar parámetros de paginación
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100; // Límite máximo

        var pagedResult = await _providerRepository.GetPagedAsync(
            page,
            pageSize,
            term,
            activeOnly: true,
            cancellationToken);

        // Mapear a ProviderDto usando el método Map de PagedResult
        return Result.Ok(pagedResult.Map(MapToProviderDto));
    }

    /// <inheritdoc />
    public async Task<Result<ProviderDto>> GetProviderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id);
        
        if (entity == null || entity.Activo != true)
            return Result.Fail<ProviderDto>("Proveedor no encontrado");

        return Result.Ok(MapToProviderDto(entity));
    }

    /// <inheritdoc />
    public async Task<Result<int>> CreateProviderAsync(CreateProviderDto dto, CancellationToken cancellationToken = default)
    {
        // Validar NIT duplicado
        if (await _providerRepository.ExistsByTaxIdAsync(dto.TaxId, cancellationToken))
        {
            return Result.Fail<int>($"El proveedor con NIT {dto.TaxId} ya existe en este condominio");
        }

        var entity = new Proveedor
        {
            RazonSocial = dto.LegalName,
            Nit = dto.TaxId,
            Contacto = dto.PhoneNumber,
            Direccion = dto.Address,
            Activo = true
        };

        await _repository.AddAsync(entity);
        return Result.Ok(entity.IdProveedor);
    }

    /// <inheritdoc />
    public async Task<Result> UpdateProviderAsync(int id, UpdateProviderDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id);
        
        if (entity == null || entity.Activo != true)
            return Result.Fail("Proveedor no encontrado");

        entity.RazonSocial = dto.LegalName;
        entity.Contacto = dto.PhoneNumber;
        entity.Direccion = dto.Address;
        // Nota: No actualizamos el NIT ya que es identificador único

        await _repository.UpdateAsync(entity);
        return Result.Ok();
    }

    /// <inheritdoc />
    public async Task<Result> DeleteProviderAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id);
        
        if (entity == null || entity.Activo != true)
            return Result.Fail("Proveedor no encontrado");

        // Soft Delete: marcar como inactivo en lugar de eliminar
        entity.Activo = false;
        await _repository.UpdateAsync(entity);

        return Result.Ok();
    }

    #endregion

    #region Mappers

    private static ProveedorDto MapToLegacyDto(Proveedor entity)
    {
        return new ProveedorDto
        {
            IdProveedor = entity.IdProveedor,
            RazonSocial = entity.RazonSocial,
            Nit = entity.Nit,
            Contacto = entity.Contacto,
            Direccion = entity.Direccion,
            Activo = entity.Activo
        };
    }

    private static ProviderDto MapToProviderDto(Proveedor entity)
    {
        return new ProviderDto
        {
            Id = entity.IdProveedor,
            TaxId = entity.Nit ?? string.Empty,
            LegalName = entity.RazonSocial,
            PhoneNumber = entity.Contacto,
            Address = entity.Direccion,
            IsActive = entity.Activo ?? false
        };
    }

    #endregion
}
