using System;
using System.Collections.Generic;

namespace Consulcon.Domain.Entities.Comunicacion;

public partial class ComunicadoBlog
{
    public int IdBlog { get; set; }

    public int IdCondominio { get; set; }

    public DateTime? FechaPublicacion { get; set; }

    public string Titulo { get; set; } = null!;

    public string? ContenidoHtml { get; set; }

    public string? UrlImagen { get; set; }

    public string? UrlArchivoAdjunto { get; set; }

    public bool? Activo { get; set; }

    public virtual Condominio IdCondominioNavigation { get; set; } = null!;
}
