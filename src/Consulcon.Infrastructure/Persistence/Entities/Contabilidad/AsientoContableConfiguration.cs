using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad;

public class AsientoContableConfiguration : IEntityTypeConfiguration<AsientoContable>
{
    public void Configure(EntityTypeBuilder<AsientoContable> builder)
    {
        builder.HasKey(e => e.IdAsiento).HasName("PRIMARY");

        builder.ToTable("asiento_contable");

        builder.HasIndex(e => e.IdCondominio, "fk_asiento_condominio");

        builder.HasIndex(e => e.IdTransaccionOrigenEgreso, "fk_asiento_egreso");

        builder.HasIndex(e => e.IdTransaccionOrigenPago, "fk_asiento_pago");

        builder.Property(e => e.IdAsiento).HasColumnName("id_asiento");
        builder.Property(e => e.FechaContable)
            .HasColumnType("datetime")
            .HasColumnName("fecha_contable");
        builder.Property(e => e.GlosaGeneral)
            .HasMaxLength(255)
            .HasColumnName("glosa_general");
        builder.Property(e => e.IdCondominio).HasColumnName("id_condominio");
        builder.Property(e => e.IdTransaccionOrigenEgreso)
            .HasComment("Link a Tesoreria")
            .HasColumnName("id_transaccion_origen_egreso");
        builder.Property(e => e.IdTransaccionOrigenPago)
            .HasComment("Link a Tesoreria")
            .HasColumnName("id_transaccion_origen_pago");
        builder.Property(e => e.NroDocumentoRespaldo)
            .HasMaxLength(50)
            .HasColumnName("nro_documento_respaldo");
        builder.Property(e => e.TipoAsiento)
            .HasMaxLength(20)
            .HasComment("Diario, Ajuste, Cierre")
            .HasColumnName("tipo_asiento");

        builder.HasOne(d => d.IdCondominioNavigation).WithMany(p => p.AsientoContables)
            .HasForeignKey(d => d.IdCondominio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_asiento_condominio");

        builder.HasOne(d => d.IdTransaccionOrigenEgresoNavigation).WithMany(p => p.AsientoContables)
            .HasForeignKey(d => d.IdTransaccionOrigenEgreso)
            .HasConstraintName("fk_asiento_egreso");

        builder.HasOne(d => d.IdTransaccionOrigenPagoNavigation).WithMany(p => p.AsientoContables)
            .HasForeignKey(d => d.IdTransaccionOrigenPago)
            .HasConstraintName("fk_asiento_pago");
    }
}
