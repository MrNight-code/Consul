using Consulcon.Domain.Entities.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.General;

public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.HasKey(e => e.IdProveedor).HasName("PRIMARY");

        builder.ToTable("proveedor");

        builder.Property(e => e.IdProveedor).HasColumnName("id_proveedor");
        builder.Property(e => e.Activo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("activo");
        builder.Property(e => e.Contacto)
            .HasMaxLength(100)
            .HasColumnName("contacto");
        builder.Property(e => e.Direccion)
            .HasMaxLength(200)
            .HasColumnName("direccion");
        builder.Property(e => e.Nit)
            .HasMaxLength(30)
            .HasColumnName("nit");
        builder.Property(e => e.RazonSocial)
            .HasMaxLength(150)
            .HasColumnName("razon_social");
    }
}
