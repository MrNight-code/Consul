using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad;

public class AsientoDetalleConfiguration : IEntityTypeConfiguration<AsientoDetalle>
{
    public void Configure(EntityTypeBuilder<AsientoDetalle> builder)
    {
        builder.HasKey(e => e.IdAsientoDet).HasName("PRIMARY");

        builder.ToTable("asiento_detalle");

        builder.HasIndex(e => e.IdAsiento, "fk_ad_asiento");

        builder.HasIndex(e => e.IdCuenta, "fk_ad_cuenta");

        builder.Property(e => e.IdAsientoDet).HasColumnName("id_asiento_det");
        builder.Property(e => e.Debe)
            .HasPrecision(12, 2)
            .HasDefaultValueSql("'0.00'")
            .HasColumnName("debe");
        builder.Property(e => e.GlosaLinea)
            .HasMaxLength(200)
            .HasColumnName("glosa_linea");
        builder.Property(e => e.Haber)
            .HasPrecision(12, 2)
            .HasDefaultValueSql("'0.00'")
            .HasColumnName("haber");
        builder.Property(e => e.IdAsiento).HasColumnName("id_asiento");
        builder.Property(e => e.IdCuenta).HasColumnName("id_cuenta");

        builder.HasOne(d => d.IdAsientoNavigation).WithMany(p => p.AsientoDetalles)
            .HasForeignKey(d => d.IdAsiento)
            .HasConstraintName("fk_ad_asiento");

        builder.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.AsientoDetalles)
            .HasForeignKey(d => d.IdCuenta)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_ad_cuenta");
    }
}
