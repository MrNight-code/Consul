using Consulcon.Domain.Entities.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Facturacion;

public class DeudaDetalleConfiguration : IEntityTypeConfiguration<DeudaDetalle>
{
    public void Configure(EntityTypeBuilder<DeudaDetalle> builder)
    {
        builder.HasKey(e => e.IdDeudaDet).HasName("PRIMARY");

        builder.ToTable("deuda_detalle");

        builder.HasIndex(e => e.IdDeuda, "fk_dd_cabecera");

        builder.HasIndex(e => e.IdServicio, "fk_dd_servicio");

        builder.Property(e => e.IdDeudaDet).HasColumnName("id_deuda_det");
        builder.Property(e => e.Cantidad)
            .HasPrecision(10, 2)
            .HasDefaultValueSql("'1.00'")
            .HasColumnName("cantidad");
        builder.Property(e => e.Concepto)
            .HasMaxLength(255)
            .HasComment("Ej: Expensa Mayo 2025")
            .HasColumnName("concepto");
        builder.Property(e => e.IdDeuda).HasColumnName("id_deuda");
        builder.Property(e => e.IdServicio)
            .HasComment("Origen del cobro")
            .HasColumnName("id_servicio");
        builder.Property(e => e.MontoUnitario)
            .HasPrecision(10, 2)
            .HasColumnName("monto_unitario");
        builder.Property(e => e.Subtotal)
            .HasPrecision(12, 2)
            .HasColumnName("subtotal");

        builder.HasOne(d => d.IdDeudaNavigation).WithMany(p => p.DeudaDetalles)
            .HasForeignKey(d => d.IdDeuda)
            .HasConstraintName("fk_dd_cabecera");

        builder.HasOne(d => d.IdServicioNavigation).WithMany(p => p.DeudaDetalles)
            .HasForeignKey(d => d.IdServicio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_dd_servicio");
    }
}
