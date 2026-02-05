namespace Consulcon.Application.DTOs.Inmuebles;

public class PropiedadDto
{
    public int Id { get; set; }
    public int IdManzano { get; set; }
    public string? ManzanoNombre { get; set; }
    public int? IdCondominio { get; set; }
    public string? CondominioNombre { get; set; }
    
    public required string CodigoUnidad { get; set; }
    public string? NombreFuncional { get; set; }
    public decimal? SuperficieM2 { get; set; }
    public decimal? PorcentajeParticipacion { get; set; }
    public decimal? ExpensaBaseDefecto { get; set; }
    public string? Tipo { get; set; }
    public bool? Activo { get; set; }
    
    /// <summary>
    /// Propietario actual de la propiedad (incluido cuando expand=owner)
    /// </summary>
    public PropietarioActualDto? PropietarioActual { get; set; }
}

/// <summary>
/// DTO para el propietario actual de una propiedad
/// </summary>
public class PropietarioActualDto
{
    public int PersonaId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Ci { get; set; }
    public DateOnly? FechaDesde { get; set; }
}

public class CreatePropiedadDto
{
    public int IdManzano { get; set; }
    public required string CodigoUnidad { get; set; }
    public string? NombreFuncional { get; set; }
    public decimal? SuperficieM2 { get; set; }
    public decimal? PorcentajeParticipacion { get; set; }
    public decimal? ExpensaBaseDefecto { get; set; }
    public string? Tipo { get; set; } // Casa, Depto, Lote
}
