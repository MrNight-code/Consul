using Consulcon.Domain.Entities.Contratos;
using Consulcon.Domain.Entities.Contabilidad; // For PlanCuenta reference in ManyToMany
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Consulcon.Infrastructure.Persistence.Entities.Contratos;

public class CatalogoServicioConfiguration : IEntityTypeConfiguration<CatalogoServicio>
{
    public void Configure(EntityTypeBuilder<CatalogoServicio> builder)
    {
        builder.HasKey(e => e.IdServicio).HasName("PRIMARY");

        builder.ToTable("catalogo_servicio");

        builder.Property(e => e.IdServicio)
            .HasComment("Antes: serviciopago")
            .HasColumnName("id_servicio");
        builder.Property(e => e.Activo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("activo");
        builder.Property(e => e.CostoBase)
            .HasPrecision(10, 2)
            .HasDefaultValueSql("'0.00'")
            .HasColumnName("costo_base");
        builder.Property(e => e.EsRecurrente)
            .HasDefaultValueSql("'1'")
            .HasColumnName("es_recurrente");
        builder.Property(e => e.Nombre)
            .HasMaxLength(100)
            .HasComment("Agua, Luz, Multa, Expensa")
            .HasColumnName("nombre");

        builder.HasMany(d => d.IdCuentaIngresos).WithMany(p => p.IdServicios)
            .UsingEntity<Dictionary<string, object>>(
                "ConfigContableServicio",
                r => r.HasOne<PlanCuenta>().WithMany()
                    .HasForeignKey("IdCuentaIngreso")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ccs_cuenta"),
                l => l.HasOne<CatalogoServicio>().WithMany()
                    .HasForeignKey("IdServicio")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ccs_servicio"),
                j =>
                {
                    j.HasKey("IdServicio", "IdCuentaIngreso")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    j.ToTable("config_contable_servicio");
                    j.HasIndex(new[] { "IdCuentaIngreso" }, "fk_ccs_cuenta");
                    j.IndexerProperty<int>("IdServicio").HasColumnName("id_servicio");
                    j.IndexerProperty<int>("IdCuentaIngreso").HasColumnName("id_cuenta_ingreso");
                });
    }
}
