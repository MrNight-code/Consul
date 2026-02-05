using Consulcon.Domain.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Seguridad;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.HasKey(e => e.IdPermiso).HasName("PRIMARY");

        builder.ToTable("permiso");

        builder.Property(e => e.IdPermiso).HasColumnName("id_permiso");
        builder.Property(e => e.Descripcion)
            .HasMaxLength(100)
            .HasComment("Antes: permiso")
            .HasColumnName("descripcion");
    }
}
