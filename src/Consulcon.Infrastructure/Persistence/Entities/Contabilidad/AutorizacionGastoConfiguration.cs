using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad;

public class AutorizacionGastoConfiguration : IEntityTypeConfiguration<AutorizacionGasto>
{
    public void Configure(EntityTypeBuilder<AutorizacionGasto> builder)
    {
        builder.HasKey(e => e.IdAutorizacion).HasName("PRIMARY");

        builder.ToTable("autorizacion_gasto");

        builder.Property(e => e.IdAutorizacion).HasColumnName("id_autorizacion");
        builder.Property(e => e.Activo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("activo");
        builder.Property(e => e.Descripcion)
            .HasMaxLength(100)
            .HasComment("Niveles de firma para gastos")
            .HasColumnName("descripcion");
    }
}
