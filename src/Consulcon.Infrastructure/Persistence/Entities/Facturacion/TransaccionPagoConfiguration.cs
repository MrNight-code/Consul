using Consulcon.Domain.Entities.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Facturacion;

public class TransaccionPagoConfiguration : IEntityTypeConfiguration<TransaccionPago>
{
    public void Configure(EntityTypeBuilder<TransaccionPago> builder)
    {
        builder.HasKey(e => e.IdPago).HasName("PRIMARY");

        builder.ToTable("transaccion_pago");

        builder.HasIndex(e => e.IdBancoDestino, "fk_tp_banco");

        builder.HasIndex(e => e.IdDeuda, "fk_tp_deuda");

        builder.HasIndex(e => e.IdFormaPago, "fk_tp_forma");

        builder.HasIndex(e => e.IdPersonaPagador, "fk_tp_persona");

        builder.Property(e => e.IdPago)
            .HasComment("Antes: cuota")
            .HasColumnName("id_pago");
        builder.Property(e => e.Estado)
            .HasMaxLength(20)
            .HasDefaultValueSql("'CONFIRMADO'")
            .HasComment("CONFIRMADO, RECHAZADO")
            .HasColumnName("estado");
        builder.Property(e => e.FechaPago)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime")
            .HasColumnName("fecha_pago");
        builder.Property(e => e.IdBancoDestino).HasColumnName("id_banco_destino");
        builder.Property(e => e.IdDeuda)
            .HasComment("Pago especifico de una deuda")
            .HasColumnName("id_deuda");
        builder.Property(e => e.IdFormaPago).HasColumnName("id_forma_pago");
        builder.Property(e => e.IdPersonaPagador).HasColumnName("id_persona_pagador");
        builder.Property(e => e.MontoAbonado)
            .HasPrecision(12, 2)
            .HasColumnName("monto_abonado");
        builder.Property(e => e.NroComprobanteBanco)
            .HasMaxLength(50)
            .HasColumnName("nro_comprobante_banco");
        builder.Property(e => e.TipoCambio)
            .HasPrecision(10, 4)
            .HasDefaultValueSql("'1.0000'")
            .HasColumnName("tipo_cambio");

        builder.Property(e => e.ReciboUrl)
            .HasMaxLength(500)
            .HasColumnName("recibo_url");

        builder.Property(e => e.FechaRecibo)
            .HasColumnName("fecha_recibo");

        builder.HasOne(d => d.IdBancoDestinoNavigation).WithMany(p => p.TransaccionPagos)
            .HasForeignKey(d => d.IdBancoDestino)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_tp_banco");

        builder.HasOne(d => d.IdDeudaNavigation).WithMany(p => p.TransaccionPagos)
            .HasForeignKey(d => d.IdDeuda)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_tp_deuda");

        builder.HasOne(d => d.IdFormaPagoNavigation).WithMany(p => p.TransaccionPagos)
            .HasForeignKey(d => d.IdFormaPago)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_tp_forma");

        builder.HasOne(d => d.IdPersonaPagadorNavigation).WithMany(p => p.TransaccionPagos)
            .HasForeignKey(d => d.IdPersonaPagador)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_tp_persona");
    }
}
