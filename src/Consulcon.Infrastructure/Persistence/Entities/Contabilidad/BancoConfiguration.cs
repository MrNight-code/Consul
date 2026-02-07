using Consulcon.Domain.Entities.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad;

public class BancoConfiguration : IEntityTypeConfiguration<Banco>
{
    public void Configure(EntityTypeBuilder<Banco> builder)
    {
        builder.HasKey(e => e.IdBanco).HasName("PRIMARY");

        builder.ToTable("banco");

        builder.HasIndex(e => e.IdCuentaContableAsociada, "fk_banco_cuenta");

        builder.Property(e => e.IdBanco).HasColumnName("id_banco");
        builder.Property(e => e.Activo)
            .HasDefaultValueSql("'1'")
            .HasColumnName("activo");
        builder.Property(e => e.IdCuentaContableAsociada).HasColumnName("id_cuenta_contable_asociada");
        builder.Property(e => e.Moneda)
            .HasMaxLength(10)
            .HasDefaultValueSql("'BOB'")
            .HasColumnName("moneda");
        builder.Property(e => e.NombreEntidad)
            .HasMaxLength(100)
            .HasColumnName("nombre_entidad");
        builder.Property(e => e.NumeroCuenta)
            .HasMaxLength(50)
            .HasColumnName("numero_cuenta");
        builder.Property(e => e.Tipo)
            .HasMaxLength(50)
            .HasDefaultValue("BANCO")
            .HasColumnName("tipo");
        builder.Property(e => e.Saldo)
            .HasPrecision(18, 2)
            .HasDefaultValue(0m)
            .HasColumnName("saldo");

        builder.HasOne(d => d.IdCuentaContableAsociadaNavigation).WithMany(p => p.Bancos)
            .HasForeignKey(d => d.IdCuentaContableAsociada)
            .HasConstraintName("fk_banco_cuenta");
    }
}
