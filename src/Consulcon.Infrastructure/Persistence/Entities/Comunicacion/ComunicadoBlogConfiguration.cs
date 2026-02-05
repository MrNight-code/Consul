using Consulcon.Domain.Entities.Comunicacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Comunicacion;

public class ComunicadoBlogConfiguration : IEntityTypeConfiguration<ComunicadoBlog>
{
    public void Configure(EntityTypeBuilder<ComunicadoBlog> builder)
    {
        builder.HasKey(e => e.IdBlog).HasName("PRIMARY");

        builder.ToTable("comunicado_blog");

        builder.HasIndex(e => e.IdCondominio, "fk_blog_condominio");

        builder.Property(e => e.IdBlog).HasColumnName("id_blog");
        builder.Property(e => e.Activo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("activo");
        builder.Property(e => e.ContenidoHtml)
            .HasColumnType("text")
            .HasColumnName("contenido_html");
        builder.Property(e => e.FechaPublicacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("fecha_publicacion");
        builder.Property(e => e.IdCondominio).HasColumnName("id_condominio");
        builder.Property(e => e.Titulo)
            .HasMaxLength(200)
            .HasColumnName("titulo");
        builder.Property(e => e.UrlArchivoAdjunto)
            .HasMaxLength(255)
            .HasColumnName("url_archivo_adjunto");
        builder.Property(e => e.UrlImagen)
            .HasMaxLength(255)
            .HasColumnName("url_imagen");

        builder.HasOne(d => d.IdCondominioNavigation).WithMany(p => p.ComunicadoBlogs)
            .HasForeignKey(d => d.IdCondominio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_blog_condominio");
    }
}
