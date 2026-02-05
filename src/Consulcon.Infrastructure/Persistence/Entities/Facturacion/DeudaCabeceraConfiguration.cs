using Consulcon.Domain.Entities.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Facturacion;

public class DeudaCabeceraConfiguration : IEntityTypeConfiguration<DeudaCabecera>
{
    public void Configure(EntityTypeBuilder<DeudaCabecera> builder)
    {
        builder.HasKey(e => e.IdDeuda).HasName("PRIMARY");

        builder.ToTable("deuda_cabecera");

        builder.HasIndex(e => e.IdContrato, "fk_deuda_contrato");

        builder.HasIndex(e => e.IdUsuarioGenerador, "fk_deuda_usuario");

        builder.Property(e => e.IdDeuda).HasColumnName("id_deuda");
        builder.Property(e => e.AnioPeriodo).HasColumnName("anio_periodo");
        builder.Property(e => e.EstadoPago)
            .HasMaxLength(20)
            .HasDefaultValueSql("'PENDIENTE'")
            .HasComment("PENDIENTE, PARCIAL, PAGADO, ANULADO")
            .HasColumnName("estado_pago");
        builder.Property(e => e.FechaEmision).HasColumnName("fecha_emision");
        builder.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
        builder.Property(e => e.IdContrato).HasColumnName("id_contrato");
        builder.Property(e => e.IdUsuarioGenerador).HasColumnName("id_usuario_generador");
        builder.Property(e => e.MesPeriodo).HasColumnName("mes_periodo");
        builder.Property(e => e.TotalDeuda)
            .HasPrecision(12, 2)
            .HasDefaultValueSql("'0.00'")
            .HasColumnName("total_deuda");
        builder.Property(e => e.TotalPagado)
            .HasPrecision(12, 2)
            .HasDefaultValueSql("'0.00'")
            .HasColumnName("total_pagado");

        builder.HasOne(d => d.IdContratoNavigation).WithMany(p => p.DeudaCabeceras)
            .HasForeignKey(d => d.IdContrato)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_deuda_contrato");

        builder.HasOne(d => d.IdUsuarioGeneradorNavigation).WithMany(p => p.DeudaCabeceras)
            .HasForeignKey(d => d.IdUsuarioGenerador)
            .HasConstraintName("fk_deuda_usuario");
    }
}
