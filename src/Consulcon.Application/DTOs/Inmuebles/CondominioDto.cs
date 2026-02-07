namespace Consulcon.Application.DTOs.Inmuebles;

public class CondominioDto
{
    public int IdCondominio { get; set; }
    public required string Nombre { get; set; }
    public string? Direccion { get; set; }
    public decimal? SuperficieTotalM2 { get; set; }
    public int IdAdminPersona { get; set; }
    public string? AdminNombre { get; set; }
    public string? ConfigDiaCobro { get; set; }
    public string? Logo { get; set; }
}
