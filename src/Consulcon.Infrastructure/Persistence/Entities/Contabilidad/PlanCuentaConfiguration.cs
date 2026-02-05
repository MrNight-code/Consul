using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad;

public class PlanCuentaConfiguration : IEntityTypeConfiguration<PlanCuenta>
{
    public void Configure(EntityTypeBuilder<PlanCuenta> builder)
    {
        builder.HasKey(e => e.IdCuenta).HasName("PRIMARY");

        builder.ToTable("plan_cuentas");

        builder.HasIndex(e => e.IdCuentaPadre, "fk_pc_padre");

        builder.Property(e => e.IdCuenta).HasColumnName("id_cuenta");
        builder.Property(e => e.CodigoCuenta)
            .HasMaxLength(20)
            .HasComment("Ej: 1.1.01")
            .HasColumnName("codigo_cuenta");
        builder.Property(e => e.EsImputable)
            .HasDefaultValueSql("'1'")
            .HasComment("Si/No")
            .HasColumnName("es_imputable");
        builder.Property(e => e.IdCuentaPadre)
            .HasComment("Recursiva")
            .HasColumnName("id_cuenta_padre");
        builder.Property(e => e.NivelJerarquia)
            .HasDefaultValueSql("'1'")
            .HasColumnName("nivel_jerarquia");
        builder.Property(e => e.Nombre)
            .HasMaxLength(100)
            .HasColumnName("nombre");

        builder.HasOne(d => d.IdCuentaPadreNavigation).WithMany(p => p.InverseIdCuentaPadreNavigation)
            .HasForeignKey(d => d.IdCuentaPadre)
            .HasConstraintName("fk_pc_padre");
    }
}
