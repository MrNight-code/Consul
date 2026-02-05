using Consulcon.Domain.Entities.Inmuebles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Inmuebles;

public class PropiedadConfiguration : IEntityTypeConfiguration<Propiedad>
{
    public void Configure(EntityTypeBuilder<Propiedad> builder)
    {
        builder.HasKey(e => e.IdPropiedad).HasName("PRIMARY");

        builder.ToTable("propiedad");

        builder.HasIndex(e => e.IdManzano, "fk_propiedad_manzano");

        builder.Property(e => e.IdPropiedad).HasColumnName("id_propiedad");
        builder.Property(e => e.Activo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("activo");
        builder.Property(e => e.CodigoUnidad)
            .HasMaxLength(20)
            .HasColumnName("codigo_unidad");
        builder.Property(e => e.ExpensaBaseDefecto)
            .HasPrecision(10, 2)
            .HasColumnName("expensa_base_defecto");
        builder.Property(e => e.IdManzano).HasColumnName("id_manzano");
        builder.Property(e => e.NombreFuncional)
            .HasMaxLength(100)
            .HasColumnName("nombre_funcional");
        builder.Property(e => e.PorcentajeParticipacion)
            .HasPrecision(5, 4)
            .HasComment("Para prorrateo")
            .HasColumnName("porcentaje_participacion");
        builder.Property(e => e.SuperficieM2)
            .HasPrecision(10, 2)
            .HasColumnName("superficie_m2");
        builder.Property(e => e.Tipo)
            .HasMaxLength(50)
            .HasComment("Casa, Depto, Lote")
            .HasColumnName("tipo");

        builder.Property(e => e.SaldoDeudor)
            .HasPrecision(12, 2)
            .HasDefaultValueSql("'0.00'")
            .HasColumnName("saldo_deudor");

        builder.HasOne(d => d.IdManzanoNavigation).WithMany(p => p.Propiedads)
            .HasForeignKey(d => d.IdManzano)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_propiedad_manzano");
    }
}
