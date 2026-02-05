namespace Consulcon.Application.DTOs.Personas;

public class MedioContactoDto
{
    public int? Id { get; set; }
    public string Tipo { get; set; } = null!;
    public string Valor { get; set; } = null!;
    public bool EsPrincipal { get; set; }
}
