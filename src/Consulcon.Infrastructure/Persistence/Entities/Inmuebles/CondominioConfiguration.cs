using Consulcon.Domain.Entities.Inmuebles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Inmuebles;

public class CondominioConfiguration : IEntityTypeConfiguration<Condominio>
{
    public void Configure(EntityTypeBuilder<Condominio> builder)
    {
        builder.HasKey(e => e.IdCondominio).HasName("PRIMARY");

        builder.ToTable("condominio");

        builder.HasIndex(e => e.IdAdminPersona, "fk_condominio_admin");

        builder.Property(e => e.IdCondominio).HasColumnName("id_condominio");
        builder.Property(e => e.Codigo)
            .HasMaxLength(20)
            .HasColumnName("codigo");
        builder.Property(e => e.ConfigDiaCobro)
            .HasMaxLength(50)
            .HasColumnName("config_dia_cobro");
        builder.Property(e => e.Direccion)
            .HasMaxLength(200)
            .HasColumnName("direccion");
        builder.Property(e => e.IdAdminPersona).HasColumnName("id_admin_persona");
        builder.Property(e => e.Logo)
            .HasMaxLength(255)
            .HasColumnName("logo");
        builder.Property(e => e.Nombre)
            .HasMaxLength(100)
            .HasColumnName("nombre");
        builder.Property(e => e.SuperficieTotalM2)
            .HasPrecision(12, 2)
            .HasColumnName("superficie_total_m2");

        builder.HasOne(d => d.IdAdminPersonaNavigation).WithMany(p => p.Condominios)
            .HasForeignKey(d => d.IdAdminPersona)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_condominio_admin");
    }
}
