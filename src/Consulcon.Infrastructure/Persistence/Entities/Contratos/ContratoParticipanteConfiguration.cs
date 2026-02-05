using Consulcon.Domain.Entities.Contratos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contratos;

public class ContratoParticipanteConfiguration : IEntityTypeConfiguration<ContratoParticipante>
{
    public void Configure(EntityTypeBuilder<ContratoParticipante> builder)
    {
        builder.HasKey(e => new { e.IdContrato, e.IdPersona })
            .HasName("PRIMARY")
            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

        builder.ToTable("contrato_participante");

        builder.HasIndex(e => e.IdPersona, "fk_cp_persona");

        builder.Property(e => e.IdContrato).HasColumnName("id_contrato");
        builder.Property(e => e.IdPersona).HasColumnName("id_persona");
        builder.Property(e => e.Activo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("activo");
        builder.Property(e => e.FechaAlta).HasColumnName("fecha_alta");
        builder.Property(e => e.FechaBaja).HasColumnName("fecha_baja");
        builder.Property(e => e.RolContrato)
            .HasMaxLength(50)
            .HasComment("Titular, Inquilino, Garante")
            .HasColumnName("rol_contrato");

        builder.HasOne(d => d.IdContratoNavigation).WithMany(p => p.ContratoParticipantes)
            .HasForeignKey(d => d.IdContrato)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_cp_contrato");

        builder.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.ContratoParticipantes)
            .HasForeignKey(d => d.IdPersona)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_cp_persona");
    }
}
