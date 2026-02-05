namespace Consulcon.Application.DTOs.Contratos;

public class CatalogoServicioDto
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public decimal? CostoBase { get; set; }
    public bool? EsRecurrente { get; set; }
    public bool? Activo { get; set; }
}
