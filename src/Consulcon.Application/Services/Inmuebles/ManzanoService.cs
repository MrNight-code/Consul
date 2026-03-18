using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Consulcon.Domain.Entities.Inmuebles;
using Consulcon.Domain.Interfaces;
using Consulcon.Domain.Common;

namespace Consulcon.Application.Services.Inmuebles;

public class ManzanoService(IRepository<Manzano> repository) : IManzanoService
{
    private readonly IRepository<Manzano> _repository = repository;

    public async Task<Result<IEnumerable<ManzanoDto>>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<ManzanoDto>>> GetByCondominioAsync(int condominioId)
    {
        var entities = await _repository.FindAsync(m => m.IdCondominio == condominioId);
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<ManzanoDto>> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<ManzanoDto>("Manzano no encontrado");
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<ManzanoDto>> CreateAsync(CreateManzanoDto dto, int condominioId)
    {
        var entity = new Manzano
        {
            IdCondominio = condominioId,
            Codigo = dto.Codigo,
            Nombre = dto.Nombre
        };

        await _repository.AddAsync(entity);
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<ManzanoDto>> UpdateAsync(int id, CreateManzanoDto dto, int condominioId)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<ManzanoDto>("Manzano no encontrado");
        
        if (entity.IdCondominio != condominioId)
            return Result.Fail<ManzanoDto>("Manzano no pertenece al condominio activo");

        entity.Codigo = dto.Codigo;
        entity.Nombre = dto.Nombre;

        await _repository.UpdateAsync(entity);
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Manzano no encontrado");

        await _repository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    private static ManzanoDto MapToDto(Manzano entity)
    {
        return new ManzanoDto
        {
            IdManzano = entity.IdManzano,
            IdCondominio = entity.IdCondominio,
            Codigo = entity.Codigo,
            Nombre = entity.Nombre
        };
    }
}
