using Consulcon.Domain.Entities.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Seguridad;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(e => e.IdUsuario).HasName("PRIMARY");

        builder.ToTable("usuario");

        builder.HasIndex(e => e.IdPersona, "fk_usuario_persona");

        builder.HasIndex(e => e.IdRolPrincipal, "fk_usuario_rol");

        builder.HasIndex(e => e.Username, "username").IsUnique();

        builder.Property(e => e.IdUsuario).HasColumnName("pk_usuario");
        builder.Property(e => e.EstaHabilitado)
            .HasDefaultValueSql("'1'")
            .HasColumnName("esta_habilitado");
        builder.Property(e => e.FechaCreacion)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("fecha_creacion");
        builder.Property(e => e.IdPersona).HasColumnName("id_persona");
        builder.Property(e => e.IdRolPrincipal).HasColumnName("id_rol_principal");
        builder.Property(e => e.PasswordHash)
            .HasMaxLength(255)
            .HasComment("Antes: contrasena")
            .HasColumnName("password_hash");
        builder.Property(e => e.Username)
            .HasMaxLength(50)
            .HasColumnName("username");

        builder.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Usuarios)
            .HasForeignKey(d => d.IdPersona)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_usuario_persona");

        builder.HasOne(d => d.IdRolPrincipalNavigation).WithMany(p => p.Usuarios)
            .HasForeignKey(d => d.IdRolPrincipal)
            .HasConstraintName("fk_usuario_rol");
    }
}
