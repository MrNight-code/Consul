using Consulcon.Application.DTOs.Personas;
using Consulcon.Application.Interfaces.Personas;
using Consulcon.Domain.Entities;
using Consulcon.Domain.Entities.General;
using Consulcon.Domain.Interfaces;

namespace Consulcon.Application.Services.Personas;

public class PersonaService(IRepository<Persona> repository) : IPersonaService
{
    private readonly IRepository<Persona> _repository = repository;

    public async Task<Result<IEnumerable<PersonaDto>>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync(includeProperties: "MedioContactos");
        return Result.Ok(entities.Select(MapToDto));
    }

    public async Task<Result<PersonaDto>> GetByIdAsync(int id)
    {
        var entities = await _repository.FindAsync(p => p.IdPersona == id, includeProperties: "MedioContactos");
        var entity = entities.FirstOrDefault();
        if (entity == null) return Result.Fail<PersonaDto>("Persona no encontrada");
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<PersonaDto>> CreateAsync(PersonaDto dto)
    {
        // Validate Uniqueness of CI
        if (!string.IsNullOrEmpty(dto.Ci))
        {
            var existing = await _repository.FindAsync(p => p.Ci == dto.Ci);
            if (existing.Any()) return Result.Fail<PersonaDto>($"El documento de identidad '{dto.Ci}' ya está registrado.");
        }

        var entity = new Persona
        {
            Ci = dto.Ci,
            NombreCompleto = dto.NombreCompleto,
            FechaNacimiento = dto.FechaNacimiento,
            Sexo = dto.Sexo,
            EstadoCivil = dto.EstadoCivil,
            IdFamiliarResponsable = dto.IdFamiliarResponsable,
            EsActivo = dto.EsActivo,
            MedioContactos =
            [
                .. (dto.MedioContactos ?? []).Select(c => new MedioContacto 
                { 
                    Tipo = c.Tipo, 
                    Valor = c.Valor, 
                    EsPrincipal = c.EsPrincipal 
                })
            ]
        };

        await _repository.AddAsync(entity);
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<PersonaDto>> UpdateAsync(int id, PersonaDto dto)
    {
        var entities = await _repository.FindAsync(p => p.IdPersona == id, includeProperties: "MedioContactos");
        var entity = entities.FirstOrDefault();
        if (entity == null) return Result.Fail<PersonaDto>("Persona no encontrada");

        // Validate Uniqueness of CI
        if (!string.IsNullOrEmpty(dto.Ci))
        {
            var existing = await _repository.FindAsync(p => p.Ci == dto.Ci && p.IdPersona != id);
            if (existing.Any()) return Result.Fail<PersonaDto>($"El documento de identidad '{dto.Ci}' ya está registrado.");
        }

        entity.Ci = dto.Ci;
        entity.NombreCompleto = dto.NombreCompleto;
        entity.FechaNacimiento = dto.FechaNacimiento;
        entity.Sexo = dto.Sexo;
        entity.EstadoCivil = dto.EstadoCivil;
        entity.IdFamiliarResponsable = dto.IdFamiliarResponsable;
        entity.EsActivo = dto.EsActivo;

        // Update Contacts (Full Replace Strategy)
        entity.MedioContactos.Clear();
        if (dto.MedioContactos != null)
        {
            foreach (var c in dto.MedioContactos)
            {
                entity.MedioContactos.Add(new MedioContacto 
                { 
                    IdPersona = entity.IdPersona, // Ensure link
                    Tipo = c.Tipo, 
                    Valor = c.Valor, 
                    EsPrincipal = c.EsPrincipal 
                });
            }
        }

        await _repository.UpdateAsync(entity);
        return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Persona no encontrada");

        await _repository.DeleteAsync(entity);
        return Result.Ok(true);
    }

    private static PersonaDto MapToDto(Persona entity)
    {
        return new PersonaDto
        {
            Id = entity.IdPersona,
            Ci = entity.Ci,
            NombreCompleto = entity.NombreCompleto,
            FechaNacimiento = entity.FechaNacimiento,
            Sexo = entity.Sexo,
            EstadoCivil = entity.EstadoCivil,
            IdFamiliarResponsable = entity.IdFamiliarResponsable,
            EsActivo = entity.EsActivo ?? true,
            MedioContactos =
            [
                .. (entity.MedioContactos ?? []).Select(c => new MedioContactoDto
                {
                    Id = c.IdContacto,
                    Tipo = c.Tipo,
                    Valor = c.Valor,
                    EsPrincipal = c.EsPrincipal ?? false
                })
            ]
        };
    }
}
