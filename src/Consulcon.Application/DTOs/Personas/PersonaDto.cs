namespace Consulcon.Application.DTOs.Personas;

public class PersonaDto
{
    public int Id { get; set; }
    public string? Ci { get; set; }
    public required string NombreCompleto { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? Sexo { get; set; }
    public string? EstadoCivil { get; set; }
    public int? IdFamiliarResponsable { get; set; }
    public bool EsActivo { get; set; } = true;
    public List<MedioContactoDto> MedioContactos { get; set; } = [];
}
