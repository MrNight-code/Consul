using Consulcon.Domain.Entities.Reservas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Reservas;

public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
{
    public void Configure(EntityTypeBuilder<Reserva> builder)
    {
        builder.HasKey(e => e.IdReserva).HasName("PRIMARY");

        builder.ToTable("reserva");

        builder.HasIndex(e => e.IdContrato, "fk_reserva_contrato");

        builder.HasIndex(e => e.IdRecurso, "fk_reserva_recurso");

        builder.Property(e => e.IdReserva)
            .HasComment("Antes: evento")
            .HasColumnName("id_reserva");
        builder.Property(e => e.AmenizadoPor)
            .HasMaxLength(100)
            .HasColumnName("amenizado_por");
        builder.Property(e => e.CantidadInvitados).HasColumnName("cantidad_invitados");
        builder.Property(e => e.Estado)
            .HasMaxLength(20)
            .HasDefaultValueSql("'PENDIENTE'")
            .HasComment("PENDIENTE, CONFIRMADA, FINALIZADA")
            .HasColumnName("estado");
        builder.Property(e => e.FechaFin)
            .HasColumnType("datetime")
            .HasColumnName("fecha_fin");
        builder.Property(e => e.FechaInicio)
            .HasColumnType("datetime")
            .HasColumnName("fecha_inicio");
        builder.Property(e => e.IdContrato)
            .HasComment("Quien reserva")
            .HasColumnName("id_contrato");
        builder.Property(e => e.IdRecurso).HasColumnName("id_recurso");
        builder.Property(e => e.MontoTotalCobrado)
            .HasPrecision(10, 2)
            .HasColumnName("monto_total_cobrado");
        builder.Property(e => e.Motivo)
            .HasMaxLength(200)
            .HasColumnName("motivo");

        builder.HasOne(d => d.IdContratoNavigation).WithMany(p => p.Reservas)
            .HasForeignKey(d => d.IdContrato)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_reserva_contrato");

        builder.HasOne(d => d.IdRecursoNavigation).WithMany(p => p.Reservas)
            .HasForeignKey(d => d.IdRecurso)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_reserva_recurso");
    }
}
