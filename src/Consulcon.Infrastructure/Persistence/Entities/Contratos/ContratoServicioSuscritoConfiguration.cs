using Consulcon.Domain.Entities.Contratos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contratos;

public class ContratoServicioSuscritoConfiguration : IEntityTypeConfiguration<ContratoServicioSuscrito>
{
    public void Configure(EntityTypeBuilder<ContratoServicioSuscrito> builder)
    {
        builder.HasKey(e => e.IdSuscripcion).HasName("PRIMARY");

        builder.ToTable("contrato_servicio_suscrito");

        builder.HasIndex(e => e.IdContrato, "fk_css_contrato");

        builder.HasIndex(e => e.IdServicio, "fk_css_servicio");

        builder.Property(e => e.IdSuscripcion)
            .HasComment("Antes: servicio_contrato")
            .HasColumnName("id_suscripcion");
        builder.Property(e => e.Activo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("activo");
        builder.Property(e => e.CostoPersonalizado)
            .HasPrecision(10, 2)
            .HasComment("Si difiere del base")
            .HasColumnName("costo_personalizado");
        builder.Property(e => e.IdContrato).HasColumnName("id_contrato");
        builder.Property(e => e.IdServicio).HasColumnName("id_servicio");

        builder.HasOne(d => d.IdContratoNavigation).WithMany(p => p.ContratoServicioSuscritos)
            .HasForeignKey(d => d.IdContrato)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_css_contrato");

        builder.HasOne(d => d.IdServicioNavigation).WithMany(p => p.ContratoServicioSuscritos)
            .HasForeignKey(d => d.IdServicio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_css_servicio");
    }
}
