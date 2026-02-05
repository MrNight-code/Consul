namespace Consulcon.Application.DTOs.Comunicacion;

public class ComunicadoBlogDto
{
    public int Id { get; set; }
    public int IdCondominio { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public string Titulo { get; set; } = null!;
    public string? ContenidoHtml { get; set; }
    public string? UrlImagen { get; set; }
    public string? UrlArchivoAdjunto { get; set; }
    public bool? Activo { get; set; }
}

public class CreateComunicadoDto
{
    public int IdCondominio { get; set; }
    public string Titulo { get; set; } = null!;
    public string? ContenidoHtml { get; set; }
    public string? UrlImagen { get; set; }
    public string? UrlArchivoAdjunto { get; set; }
}
