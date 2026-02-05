using Consulcon.Domain.Entities.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Facturacion;

public class LecturaServicioConfiguration : IEntityTypeConfiguration<LecturaServicio>
{
    public void Configure(EntityTypeBuilder<LecturaServicio> builder)
    {
        builder.HasKey(e => e.IdLectura).HasName("PRIMARY");

        builder.ToTable("lectura_servicio");

        builder.HasIndex(e => e.IdSuscripcion, "fk_lectura_suscripcion");

        builder.Property(e => e.IdLectura).HasColumnName("id_lectura");
        builder.Property(e => e.Anio).HasColumnName("anio");
        builder.Property(e => e.FechaLectura).HasColumnName("fecha_lectura");
        builder.Property(e => e.IdSuscripcion).HasColumnName("id_suscripcion");
        builder.Property(e => e.Mes).HasColumnName("mes");
        builder.Property(e => e.MontoCalculado)
            .HasPrecision(10, 2)
            .HasColumnName("monto_calculado");
        builder.Property(e => e.ValorLeido)
            .HasPrecision(12, 2)
            .HasComment("Para agua/luz variable")
            .HasColumnName("valor_leido");

        builder.HasOne(d => d.IdSuscripcionNavigation).WithMany(p => p.LecturaServicios)
            .HasForeignKey(d => d.IdSuscripcion)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_lectura_suscripcion");
    }
}
