using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad;

public class EgresoDetalleConfiguration : IEntityTypeConfiguration<EgresoDetalle>
{
    public void Configure(EntityTypeBuilder<EgresoDetalle> builder)
    {
        builder.HasKey(e => e.IdEgresoDetalle).HasName("PRIMARY");

        builder.ToTable("egreso_detalle");

        builder.HasIndex(e => e.IdEgreso, "fk_egreso_detalle_egreso");

        builder.Property(e => e.IdEgresoDetalle).HasColumnName("id_egreso_detalle");
        builder.Property(e => e.IdEgreso).HasColumnName("id_egreso");
        builder.Property(e => e.Concepto)
            .HasMaxLength(255)
            .HasColumnName("concepto");
        builder.Property(e => e.Cantidad).HasColumnName("cantidad");
        builder.Property(e => e.PrecioUnitario)
            .HasPrecision(12, 2)
            .HasColumnName("precio_unitario");
        builder.Property(e => e.Subtotal)
            .HasPrecision(12, 2)
            .HasColumnName("subtotal");

        builder.HasOne(d => d.IdEgresoNavigation).WithMany(p => p.EgresoDetalles)
            .HasForeignKey(d => d.IdEgreso)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_egreso_detalle_egreso");
    }
}
