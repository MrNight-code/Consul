using Consulcon.Domain.Entities.Facturacion;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Facturacion;

public class FormaPagoConfiguration : IEntityTypeConfiguration<FormaPago>
{
    public void Configure(EntityTypeBuilder<FormaPago> builder)
    {
        builder.HasKey(e => e.IdFormaPago).HasName("PRIMARY");

        builder.ToTable("forma_pago");

        builder.HasIndex(e => e.IdCuentaContableAsociada, "fk_fp_cuenta");

        builder.Property(e => e.IdFormaPago).HasColumnName("id_forma_pago");
        builder.Property(e => e.Descripcion)
            .HasMaxLength(50)
            .HasComment("Efectivo, Cheque, Transferencia")
            .HasColumnName("descripcion");
        builder.Property(e => e.IdCuentaContableAsociada).HasColumnName("id_cuenta_contable_asociada");

        builder.HasOne(d => d.IdCuentaContableAsociadaNavigation).WithMany(p => p.FormaPagos)
            .HasForeignKey(d => d.IdCuentaContableAsociada)
            .HasConstraintName("fk_fp_cuenta");
    }
}
