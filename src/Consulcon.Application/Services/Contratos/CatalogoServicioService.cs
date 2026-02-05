using Consulcon.Application.DTOs.Contratos;
using Consulcon.Application.Interfaces.Contratos;

namespace Consulcon.Application.Services.Contratos;

public class CatalogoServicioService : ICatalogoServicioService
{
    private readonly IRepository<CatalogoServicio> _repository;

    public CatalogoServicioService(IRepository<CatalogoServicio> repository)
    {
        _repository = repository;
    }

    public async Task<Result<IEnumerable<CatalogoServicioDto>>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        var dtos = entities.Select(MapToDto);
        return Result.Ok(dtos);
    }

    public async Task<Result<CatalogoServicioDto>> CreateAsync(CatalogoServicioDto dto)
    {
        var entity = new CatalogoServicio
        {
            Nombre = dto.Nombre,
            CostoBase = dto.CostoBase,
            EsRecurrente = dto.EsRecurrente,
            Activo = dto.Activo ?? true
        };

        await _repository.AddAsync(entity);
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<CatalogoServicioDto>> UpdateAsync(int id, CatalogoServicioDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<CatalogoServicioDto>("Servicio no encontrado");

        entity.Nombre = dto.Nombre;
        entity.CostoBase = dto.CostoBase;
        entity.EsRecurrente = dto.EsRecurrente;
        entity.Activo = dto.Activo;

        await _repository.UpdateAsync(entity);
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Servicio no encontrado");

        await _repository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    private static CatalogoServicioDto MapToDto(CatalogoServicio entity)
    {
        return new CatalogoServicioDto
        {
            Id = entity.IdServicio,
            Nombre = entity.Nombre,
            CostoBase = entity.CostoBase,
            EsRecurrente = entity.EsRecurrente,
            Activo = entity.Activo
        };
    }
}
