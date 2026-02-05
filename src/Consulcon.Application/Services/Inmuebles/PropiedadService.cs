using Consulcon.Application.DTOs.Inmuebles;
using Consulcon.Application.Interfaces.Inmuebles;
using Consulcon.Domain.Constants;

namespace Consulcon.Application.Services.Inmuebles;

public class PropiedadService(IRepository<Propiedad> repository, IRepository<Manzano> manzanoRepository) : IPropiedadService
{
    private readonly IRepository<Propiedad> _repository = repository;
    private readonly IRepository<Manzano> _manzanoRepository = manzanoRepository;

    public async Task<Result<IEnumerable<PropiedadDto>>> GetAllAsync(string[]? expand = null)
    {
        var includeProperties = BuildIncludeProperties(expand);
        var entities = await _repository.GetAllAsync(includeProperties: includeProperties);
        var dtos = entities.Select(e => MapToDto(e, expand ?? []));
        return Result.Ok(dtos);
    }

    public async Task<Result<IEnumerable<PropiedadDto>>> GetByCondominioAsync(int condominioId, string[]? expand = null)
    {
        var includeProperties = BuildIncludeProperties(expand);
        var entities = await _repository.FindAsync(
            p => p.IdManzanoNavigation.IdCondominio == condominioId,
            includeProperties: includeProperties);
        var dtos = entities.Select(e => MapToDto(e, expand ?? []));
        return Result.Ok(dtos);
    }

    public async Task<Result<PropiedadDto>> GetByIdAsync(int id, string[]? expand = null)
    {
        var includeProperties = BuildIncludeProperties(expand);
        var entities = await _repository.FindAsync(
            p => p.IdPropiedad == id,
            includeProperties: includeProperties);
        var entity = entities.FirstOrDefault();

        if (entity == null) return Result.Fail<PropiedadDto>("Propiedad no encontrada");

        return Result.Ok(MapToDto(entity, expand ?? []));
    }

    private static string BuildIncludeProperties(string[]? expand)
    {
        var includes = new List<string>
        {
            "IdManzanoNavigation",
            "IdManzanoNavigation.IdCondominioNavigation"
        };

        // Si se solicita expandir owner, incluir las relaciones necesarias
        if (expand?.Contains("owner", StringComparer.OrdinalIgnoreCase) == true)
        {
            includes.Add("Contratos.ContratoParticipantes.IdPersonaNavigation");
        }

        return string.Join(",", includes);
    }

    public async Task<Result<PropiedadDto>> CreateAsync(CreatePropiedadDto dto)
    {
        // Validate Manzano and Uniqueness
        var manzano = await _manzanoRepository.GetByIdAsync(dto.IdManzano);
        if (manzano == null) return Result.Fail<PropiedadDto>("El Manzano especificado no existe.");

        var existing = await _repository.FindAsync(p => p.CodigoUnidad == dto.CodigoUnidad && p.IdManzanoNavigation.IdCondominio == manzano.IdCondominio);
        if (existing.Any()) return Result.Fail<PropiedadDto>($"La unidad '{dto.CodigoUnidad}' ya existe en este condominio.");

        var entity = new Propiedad
        {
            IdManzano = dto.IdManzano,
            CodigoUnidad = dto.CodigoUnidad,
            NombreFuncional = dto.NombreFuncional,
            SuperficieM2 = dto.SuperficieM2,
            PorcentajeParticipacion = dto.PorcentajeParticipacion,
            ExpensaBaseDefecto = dto.ExpensaBaseDefecto,
            Tipo = dto.Tipo,
            Activo = true
        };

        await _repository.AddAsync(entity);
        
        // Re-fetch to get navigation properties for the DTO return (optional, or just return basic)
        // For efficiency we might skip re-fetch or do a specific get. 
        // Here assuming ID is populated after AddAsync (EfCore does this).
        
        return await GetByIdAsync(entity.IdPropiedad);
    }

    public async Task<Result<PropiedadDto>> UpdateAsync(int id, CreatePropiedadDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<PropiedadDto>("Propiedad no encontrada");

        // Validate Uniqueness against NEW Manzano (if changed) or SAME Condominio
        // First get the target Manzano to know the Condominio
        var targetManzano = await _manzanoRepository.GetByIdAsync(dto.IdManzano);
        if (targetManzano == null) return Result.Fail<PropiedadDto>("El Manzano especificado no existe.");

        var existing = await _repository.FindAsync(p => 
            p.CodigoUnidad == dto.CodigoUnidad && 
            p.IdManzanoNavigation.IdCondominio == targetManzano.IdCondominio && 
            p.IdPropiedad != id);
            
        if (existing.Any()) return Result.Fail<PropiedadDto>($"La unidad '{dto.CodigoUnidad}' ya existe en este condominio.");

        entity.IdManzano = dto.IdManzano;
        entity.CodigoUnidad = dto.CodigoUnidad;
        entity.NombreFuncional = dto.NombreFuncional;
        entity.SuperficieM2 = dto.SuperficieM2;
        entity.PorcentajeParticipacion = dto.PorcentajeParticipacion;
        entity.ExpensaBaseDefecto = dto.ExpensaBaseDefecto;
        entity.Tipo = dto.Tipo;

        await _repository.UpdateAsync(entity);
        
        return await GetByIdAsync(id);
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Propiedad no encontrada");

        await _repository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    private static PropiedadDto MapToDto(Propiedad entity, string[] expand)
    {
        var dto = new PropiedadDto
        {
            Id = entity.IdPropiedad,
            IdManzano = entity.IdManzano,
            ManzanoNombre = entity.IdManzanoNavigation?.Nombre,
            IdCondominio = entity.IdManzanoNavigation?.IdCondominio,
            CondominioNombre = entity.IdManzanoNavigation?.IdCondominioNavigation?.Nombre,
            CodigoUnidad = entity.CodigoUnidad,
            NombreFuncional = entity.NombreFuncional,
            SuperficieM2 = entity.SuperficieM2,
            PorcentajeParticipacion = entity.PorcentajeParticipacion,
            ExpensaBaseDefecto = entity.ExpensaBaseDefecto,
            Tipo = entity.Tipo,
            Activo = entity.Activo
        };

        // Solo buscar propietario si se solicitó en expand
        if (expand.Contains("owner", StringComparer.OrdinalIgnoreCase))
        {
            var propietarioActual = entity.Contratos
                .Where(c => c.Estado == OwnershipConstants.EstadoVigente || c.Estado == null)
                .SelectMany(c => c.ContratoParticipantes)
                .FirstOrDefault(cp =>
                    cp.RolContrato == OwnershipConstants.RolTitular &&
                    cp.Activo == true &&
                    cp.FechaBaja == null);

            if (propietarioActual != null)
            {
                dto.PropietarioActual = new PropietarioActualDto
                {
                    PersonaId = propietarioActual.IdPersona,
                    NombreCompleto = propietarioActual.IdPersonaNavigation?.NombreCompleto ?? "Desconocido",
                    Ci = propietarioActual.IdPersonaNavigation?.Ci,
                    FechaDesde = propietarioActual.FechaAlta
                };
            }
        }

        return dto;
    }
}
