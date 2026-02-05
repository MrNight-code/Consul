using Consulcon.Domain.Entities.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.General;

public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.HasKey(e => e.IdPersona).HasName("PRIMARY");

        builder.ToTable("persona");

        builder.HasIndex(e => e.IdFamiliarResponsable, "fk_persona_familiar");

        builder.Property(e => e.IdPersona).HasColumnName("id_persona");
        builder.Property(e => e.Ci)
            .HasMaxLength(20)
            .HasColumnName("ci");
        builder.Property(e => e.EsActivo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("es_activo");
        builder.Property(e => e.EstadoCivil)
            .HasMaxLength(20)
            .HasColumnName("estado_civil");
        builder.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
        builder.Property(e => e.IdFamiliarResponsable)
            .HasComment("Recursiva: Para hijos/dependientes")
            .HasColumnName("id_familiar_responsable");
        builder.Property(e => e.NombreCompleto)
            .HasMaxLength(150)
            .HasComment("Antes: nombre")
            .HasColumnName("nombre_completo");
        builder.Property(e => e.Sexo)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("sexo");

        builder.HasOne(d => d.IdFamiliarResponsableNavigation).WithMany(p => p.InverseIdFamiliarResponsableNavigation)
            .HasForeignKey(d => d.IdFamiliarResponsable)
            .HasConstraintName("fk_persona_familiar");
    }
}
