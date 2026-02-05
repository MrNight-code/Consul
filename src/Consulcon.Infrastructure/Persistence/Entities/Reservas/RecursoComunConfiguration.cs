using Consulcon.Domain.Entities.Reservas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Reservas;

public class RecursoComunConfiguration : IEntityTypeConfiguration<RecursoComun>
{
    public void Configure(EntityTypeBuilder<RecursoComun> builder)
    {
        builder.HasKey(e => e.IdRecurso).HasName("PRIMARY");

        builder.ToTable("recurso_comun");

        builder.HasIndex(e => e.IdCondominio, "fk_recurso_condominio");

        builder.Property(e => e.IdRecurso).HasColumnName("id_recurso");
        builder.Property(e => e.ColorCalendario)
            .HasMaxLength(20)
            .HasComment("Antes en tabla evento")
            .HasColumnName("color_calendario");
        builder.Property(e => e.CostoGarantia)
            .HasPrecision(10, 2)
            .HasDefaultValueSql("'0.00'")
            .HasColumnName("costo_garantia");
        builder.Property(e => e.CostoReserva)
            .HasPrecision(10, 2)
            .HasDefaultValueSql("'0.00'")
            .HasColumnName("costo_reserva");
        builder.Property(e => e.IdCondominio).HasColumnName("id_condominio");
        builder.Property(e => e.Nombre)
            .HasMaxLength(100)
            .HasComment("Churrasquera, Salon")
            .HasColumnName("nombre");

        builder.HasOne(d => d.IdCondominioNavigation).WithMany(p => p.RecursoComuns)
            .HasForeignKey(d => d.IdCondominio)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_recurso_condominio");
    }
}
