using Consulcon.Domain.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Consulcon.Infrastructure.Persistence.Entities.Seguridad;

public class RolConfiguration : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> builder)
    {
        builder.HasKey(e => e.IdRol).HasName("PRIMARY");

        builder.ToTable("rol");

        builder.Property(e => e.IdRol)
            .HasComment("Antes: tipousuario")
            .HasColumnName("id_rol");
        builder.Property(e => e.Nombre)
            .HasMaxLength(50)
            .HasComment("Admin, Guardia, Vecino")
            .HasColumnName("nombre");

        builder.HasMany(d => d.IdPermisos).WithMany(p => p.IdRols)
            .UsingEntity<Dictionary<string, object>>(
                "RolPermiso",
                r => r.HasOne<Permiso>().WithMany()
                    .HasForeignKey("IdPermiso")
                    .HasConstraintName("fk_rp_permiso"),
                l => l.HasOne<Rol>().WithMany()
                    .HasForeignKey("IdRol")
                    .HasConstraintName("fk_rp_rol"),
                j =>
                {
                    j.HasKey("IdRol", "IdPermiso")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    j.ToTable("rol_permiso");
                    j.HasIndex(new[] { "IdPermiso" }, "fk_rp_permiso");
                    j.IndexerProperty<int>("IdRol").HasColumnName("id_rol");
                    j.IndexerProperty<int>("IdPermiso").HasColumnName("id_permiso");
                });
    }
}
