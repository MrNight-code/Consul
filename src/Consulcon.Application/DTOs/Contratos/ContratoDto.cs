namespace Consulcon.Application.DTOs.Contratos;

public class ContratoDto
{
    public int Id { get; set; }
    public int IdPropiedad { get; set; }
    public string? PropiedadNombre { get; set; } // e.g. "Depto 101"
    public DateOnly? FechaFirma { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public DateOnly? FechaIngresoReal { get; set; }
    public decimal MontoExpensaPactada { get; set; }
    public string? Estado { get; set; }
    public string? MotivoBaja { get; set; }
    public int? IdUsuarioCreador { get; set; }
    
    public List<ContratoParticipanteDto> Participantes { get; set; } = new();
}

public class CreateContratoDto
{
    public int IdPropiedad { get; set; }
    public DateOnly? FechaFirma { get; set; }
    public required DateOnly FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public DateOnly? FechaIngresoReal { get; set; }
    public decimal MontoExpensaPactada { get; set; }
    public int IdUsuarioCreador { get; set; }
    
    public List<CreateContratoParticipanteDto> Participantes { get; set; } = new();
}

public class ContratoParticipanteDto
{
    public int IdPersona { get; set; }
    public string? PersonaNombre { get; set; }
    public string RolContrato { get; set; } = null!;
    public DateOnly? FechaAlta { get; set; }
    public bool? Activo { get; set; }
}

public class CreateContratoParticipanteDto
{
    public int IdPersona { get; set; }
    public required string RolContrato { get; set; } // Titular, Inquilino, Garante
}
