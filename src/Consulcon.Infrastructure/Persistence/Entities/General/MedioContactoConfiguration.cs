using Consulcon.Domain.Entities.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.General;

public class MedioContactoConfiguration : IEntityTypeConfiguration<MedioContacto>
{
    public void Configure(EntityTypeBuilder<MedioContacto> builder)
    {
        builder.HasKey(e => e.IdContacto).HasName("PRIMARY");

        builder.ToTable("medio_contacto");

        builder.HasIndex(e => e.IdPersona, "fk_contacto_persona");

        builder.Property(e => e.IdContacto).HasColumnName("id_contacto");
        builder.Property(e => e.EsPrincipal)
            .HasDefaultValueSql("'0'")
            .HasColumnName("es_principal");
        builder.Property(e => e.IdPersona).HasColumnName("id_persona");
        builder.Property(e => e.Tipo)
            .HasMaxLength(50)
            .HasComment("Telefono, Celular, Email, Facebook")
            .HasColumnName("tipo");
        builder.Property(e => e.Valor)
            .HasMaxLength(100)
            .HasComment("El numero o correo")
            .HasColumnName("valor");

        builder.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.MedioContactos)
            .HasForeignKey(d => d.IdPersona)
            .HasConstraintName("fk_contacto_persona");
    }
}
