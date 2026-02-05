using Consulcon.Domain.Entities.Financiero;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Financiero;

public class FinancialConfigConfiguration : IEntityTypeConfiguration<FinancialConfig>
{
    public void Configure(EntityTypeBuilder<FinancialConfig> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.ToTable("financial_config");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CondominiumId).HasColumnName("condominium_id");
        builder.Property(e => e.MonthlyInterestRate)
            .HasPrecision(5, 2)
            .HasColumnName("monthly_interest_rate");
        builder.Property(e => e.GraceDays).HasColumnName("grace_days");

        builder.HasOne(d => d.Condominium)
            .WithMany() // Assuming no collection in Condominio for this yet
            .HasForeignKey(d => d.CondominiumId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
