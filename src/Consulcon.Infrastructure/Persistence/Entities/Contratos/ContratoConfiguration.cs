using Consulcon.Domain.Entities.Contratos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contratos;

public class ContratoConfiguration : IEntityTypeConfiguration<Contrato>
{
    public void Configure(EntityTypeBuilder<Contrato> builder)
    {
        builder.HasKey(e => e.IdContrato).HasName("PRIMARY");

        builder.ToTable("contrato");

        builder.HasIndex(e => e.IdUsuarioCreador, "fk_contrato_creador");

        builder.HasIndex(e => e.IdPropiedad, "fk_contrato_propiedad");

        builder.Property(e => e.IdContrato).HasColumnName("id_contrato");
        builder.Property(e => e.Estado)
            .HasMaxLength(20)
            .HasDefaultValueSql("'Vigente'")
            .HasComment("Vigente, Finalizado, Rescindido")
            .HasColumnName("estado");
        builder.Property(e => e.FechaFin).HasColumnName("fecha_fin");
        builder.Property(e => e.FechaFirma).HasColumnName("fecha_firma");
        builder.Property(e => e.FechaIngresoReal).HasColumnName("fecha_ingreso_real");
        builder.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
        builder.Property(e => e.IdPropiedad).HasColumnName("id_propiedad");
        builder.Property(e => e.IdUsuarioCreador).HasColumnName("id_usuario_creador");
        builder.Property(e => e.MontoExpensaPactada)
            .HasPrecision(10, 2)
            .HasColumnName("monto_expensa_pactada");
        builder.Property(e => e.MotivoBaja)
            .HasMaxLength(255)
            .HasColumnName("motivo_baja");

        builder.HasOne(d => d.IdPropiedadNavigation).WithMany(p => p.Contratos)
            .HasForeignKey(d => d.IdPropiedad)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_contrato_propiedad");

        builder.HasOne(d => d.IdUsuarioCreadorNavigation).WithMany(p => p.Contratos)
            .HasForeignKey(d => d.IdUsuarioCreador)
            .HasConstraintName("fk_contrato_creador");
    }
}
