using Consulcon.Domain.Entities.Inmuebles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Inmuebles;

public class ManzanoConfiguration : IEntityTypeConfiguration<Manzano>
{
    public void Configure(EntityTypeBuilder<Manzano> builder)
    {
        builder.HasKey(e => e.IdManzano).HasName("PRIMARY");

        builder.ToTable("manzano");

        builder.HasIndex(e => e.IdCondominio, "fk_manzano_condominio");

        builder.Property(e => e.IdManzano).HasColumnName("id_manzano");
        builder.Property(e => e.Codigo)
            .HasMaxLength(20)
            .HasColumnName("codigo");
        builder.Property(e => e.IdCondominio).HasColumnName("id_condominio");
        builder.Property(e => e.Nombre)
            .HasMaxLength(50)
            .HasColumnName("nombre");

        builder.HasOne(d => d.IdCondominioNavigation).WithMany(p => p.Manzanos)
            .HasForeignKey(d => d.IdCondominio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_manzano_condominio");
    }
}
