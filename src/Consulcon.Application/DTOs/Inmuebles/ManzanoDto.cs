namespace Consulcon.Application.DTOs.Inmuebles;

public class ManzanoDto
{
    public int IdManzano { get; set; }
    public int IdCondominio { get; set; }
    public string Codigo { get; set; } = null!;
    public string? Nombre { get; set; }
}
