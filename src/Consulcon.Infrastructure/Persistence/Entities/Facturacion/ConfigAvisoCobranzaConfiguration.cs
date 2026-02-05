using Consulcon.Domain.Entities.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Facturacion;

public class ConfigAvisoCobranzaConfiguration : IEntityTypeConfiguration<ConfigAvisoCobranza>
{
    public void Configure(EntityTypeBuilder<ConfigAvisoCobranza> builder)
    {
        builder.HasKey(e => e.IdConfig).HasName("PRIMARY");

        builder.ToTable("config_aviso_cobranza");

        builder.HasIndex(e => e.IdCondominio, "fk_aviso_condominio");

        builder.Property(e => e.IdConfig)
            .HasComment("Antes: confaviso")
            .HasColumnName("id_config");
        builder.Property(e => e.DiasVencimientoDefecto)
            .HasDefaultValueSql("'10'")
            .HasColumnName("dias_vencimiento_defecto");
        builder.Property(e => e.IdCondominio).HasColumnName("id_condominio");
        builder.Property(e => e.TextoFooter)
            .HasColumnType("text")
            .HasColumnName("texto_footer");
        builder.Property(e => e.TextoHeader)
            .HasColumnType("text")
            .HasColumnName("texto_header");

        builder.HasOne(d => d.IdCondominioNavigation).WithMany(p => p.ConfigAvisoCobranzas)
            .HasForeignKey(d => d.IdCondominio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_aviso_condominio");
    }
}
