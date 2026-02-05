using Consulcon.Domain.Entities.Financiero;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Financiero;

public class ChargeConceptConfiguration : IEntityTypeConfiguration<ChargeConcept>
{
    public void Configure(EntityTypeBuilder<ChargeConcept> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.ToTable("charge_concept");

        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.CondominiumId).HasColumnName("condominium_id");
        builder.Property(e => e.Name)
            .HasMaxLength(150)
            .HasColumnName("name");
        builder.Property(e => e.Code)
            .HasMaxLength(50)
            .HasColumnName("code");
        builder.Property(e => e.IsRecurrent).HasColumnName("is_recurrent");
        builder.Property(e => e.IsActive).HasColumnName("is_active");

        builder.HasOne(d => d.Condominium)
            .WithMany() // Assuming no collection in Condominio for this yet
            .HasForeignKey(d => d.CondominiumId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
