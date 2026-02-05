using Consulcon.Application.DTOs.Contratos;
using Consulcon.Application.Interfaces.Contratos;

namespace Consulcon.Application.Services.Contratos;

public class ContratoService : IContratoService
{
    private readonly IRepository<Contrato> _contratoRepository;
    private readonly IRepository<ContratoParticipante> _participanteRepository;

    public ContratoService(IRepository<Contrato> contratoRepository, IRepository<ContratoParticipante> participanteRepository)
    {
        _contratoRepository = contratoRepository;
        _participanteRepository = participanteRepository;
    }

    public async Task<Result<IEnumerable<ContratoDto>>> GetAllAsync()
    {
        var entities = await _contratoRepository.GetAllAsync(includeProperties: "IdPropiedadNavigation,ContratoParticipantes,ContratoParticipantes.IdPersonaNavigation");
        var dtos = entities.Select(MapToDto);
        return Result.Ok(dtos);
    }

    public async Task<Result<IEnumerable<ContratoDto>>> GetByPropiedadAsync(int propiedadId)
    {
        var entities = await _contratoRepository.FindAsync(c => c.IdPropiedad == propiedadId, 
                                                           includeProperties: "IdPropiedadNavigation,ContratoParticipantes,ContratoParticipantes.IdPersonaNavigation");
        var dtos = entities.Select(MapToDto);
        return Result.Ok(dtos);
    }

    public async Task<Result<ContratoDto>> GetByIdAsync(int id)
    {
         var entities = await _contratoRepository.FindAsync(c => c.IdContrato == id, 
                                                           includeProperties: "IdPropiedadNavigation,ContratoParticipantes,ContratoParticipantes.IdPersonaNavigation");
         var entity = entities.FirstOrDefault();
         if (entity == null) return Result.Fail<ContratoDto>("Contrato no encontrado");

         return Result.Ok(MapToDto(entity));
    }

    public async Task<Result<ContratoDto>> CreateAsync(CreateContratoDto dto)
    {
        // Logic: Create Contract first
        var contrato = new Contrato
        {
            IdPropiedad = dto.IdPropiedad,
            FechaFirma = dto.FechaFirma,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            FechaIngresoReal = dto.FechaIngresoReal,
            MontoExpensaPactada = dto.MontoExpensaPactada,
            IdUsuarioCreador = dto.IdUsuarioCreador,
            Estado = "Vigente"
        };

        await _contratoRepository.AddAsync(contrato);

        // Add Participants
        if (dto.Participantes != null && dto.Participantes.Any())
        {
            foreach (var p in dto.Participantes)
            {
                var participante = new ContratoParticipante
                {
                    IdContrato = contrato.IdContrato,
                    IdPersona = p.IdPersona,
                    RolContrato = p.RolContrato,
                    FechaAlta = DateOnly.FromDateTime(DateTime.Now),
                    Activo = true
                };
                await _participanteRepository.AddAsync(participante);
            }
        }

        return await GetByIdAsync(contrato.IdContrato);
    }

    public async Task<Result<ContratoDto>> AddParticipanteAsync(int contratoId, CreateContratoParticipanteDto dto)
    {
         var participante = new ContratoParticipante
        {
            IdContrato = contratoId,
            IdPersona = dto.IdPersona,
            RolContrato = dto.RolContrato,
            FechaAlta = DateOnly.FromDateTime(DateTime.Now),
            Activo = true
        };
        await _participanteRepository.AddAsync(participante);
        
        return await GetByIdAsync(contratoId);
    }

    public async Task<Result<bool>> TerminateAsync(int id, string motivo, DateOnly fechaFin)
    {
        var entity = await _contratoRepository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Contrato no encontrado");

        entity.Estado = "Finalizado";
        entity.MotivoBaja = motivo;
        entity.FechaFin = fechaFin;
        // Also logic to deactivate participants? Maybe later.

        await _contratoRepository.UpdateAsync(entity);
        return Result.Ok(true);
    }

    private static ContratoDto MapToDto(Contrato entity)
    {
        return new ContratoDto
        {
            Id = entity.IdContrato,
            IdPropiedad = entity.IdPropiedad,
            PropiedadNombre = entity.IdPropiedadNavigation?.NombreFuncional ?? entity.IdPropiedadNavigation?.CodigoUnidad,
            FechaFirma = entity.FechaFirma,
            FechaInicio = entity.FechaInicio,
            FechaFin = entity.FechaFin,
            FechaIngresoReal = entity.FechaIngresoReal,
            MontoExpensaPactada = entity.MontoExpensaPactada,
            Estado = entity.Estado,
            MotivoBaja = entity.MotivoBaja,
            IdUsuarioCreador = entity.IdUsuarioCreador,
            Participantes = entity.ContratoParticipantes.Select(p => new ContratoParticipanteDto
            {
                IdPersona = p.IdPersona,
                PersonaNombre = p.IdPersonaNavigation?.NombreCompleto,
                RolContrato = p.RolContrato,
                FechaAlta = p.FechaAlta,
                Activo = p.Activo
            }).ToList()
        };
    }
}
