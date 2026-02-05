using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad;

public class EgresoConfiguration : IEntityTypeConfiguration<Egreso>
{
    public void Configure(EntityTypeBuilder<Egreso> builder)
    {
        builder.HasKey(e => e.IdEgreso).HasName("PRIMARY");

        builder.ToTable("egreso");

        builder.HasIndex(e => e.IdAutorizacion, "fk_egreso_aut");

        builder.HasIndex(e => e.IdBancoOrigen, "fk_egreso_banco");

        builder.HasIndex(e => e.IdCondominio, "fk_egreso_condominio");

        builder.HasIndex(e => e.IdFormaPago, "fk_egreso_fp");

        builder.HasIndex(e => e.IdPersonaBeneficiario, "fk_egreso_persona");

        builder.HasIndex(e => e.IdProveedor, "fk_egreso_proveedor");

        builder.HasIndex(e => e.IdUsuarioRegistro, "fk_egreso_usuario");

        builder.Property(e => e.IdEgreso).HasColumnName("id_egreso");
        builder.Property(e => e.Concepto)
            .HasMaxLength(300)
            .HasColumnName("concepto");
        builder.Property(e => e.FechaEgreso)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("fecha_egreso");
        builder.Property(e => e.IdAutorizacion).HasColumnName("id_autorizacion");
        builder.Property(e => e.IdBancoOrigen).HasColumnName("id_banco_origen");
        builder.Property(e => e.IdCondominio).HasColumnName("id_condominio");
        builder.Property(e => e.IdFormaPago).HasColumnName("id_forma_pago");
        builder.Property(e => e.IdPersonaBeneficiario)
            .HasComment("Opcional")
            .HasColumnName("id_persona_beneficiario");
        builder.Property(e => e.IdProveedor)
            .HasComment("Opcional")
            .HasColumnName("id_proveedor");
        builder.Property(e => e.IdUsuarioRegistro).HasColumnName("id_usuario_registro");
        builder.Property(e => e.MontoTotal)
            .HasPrecision(12, 2)
            .HasColumnName("monto_total");
        builder.Property(e => e.NroFacturaProveedor)
            .HasMaxLength(50)
            .HasColumnName("nro_factura_proveedor");

        builder.HasOne(d => d.IdAutorizacionNavigation).WithMany(p => p.Egresos)
            .HasForeignKey(d => d.IdAutorizacion)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_egreso_aut");

        builder.HasOne(d => d.IdBancoOrigenNavigation).WithMany(p => p.Egresos)
            .HasForeignKey(d => d.IdBancoOrigen)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_egreso_banco");

        builder.HasOne(d => d.IdCondominioNavigation).WithMany(p => p.Egresos)
            .HasForeignKey(d => d.IdCondominio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_egreso_condominio");

        builder.HasOne(d => d.IdFormaPagoNavigation).WithMany(p => p.Egresos)
            .HasForeignKey(d => d.IdFormaPago)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_egreso_fp");

        builder.HasOne(d => d.IdPersonaBeneficiarioNavigation).WithMany(p => p.Egresos)
            .HasForeignKey(d => d.IdPersonaBeneficiario)
            .HasConstraintName("fk_egreso_persona");

        builder.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Egresos)
            .HasForeignKey(d => d.IdProveedor)
            .HasConstraintName("fk_egreso_proveedor");

        builder.HasOne(d => d.IdUsuarioRegistroNavigation).WithMany(p => p.Egresos)
            .HasForeignKey(d => d.IdUsuarioRegistro)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_egreso_usuario");
    }
}
