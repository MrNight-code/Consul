using Consulcon.Domain.Entities.Contabilidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad;

public class ExpenseAttachmentConfiguration : IEntityTypeConfiguration<ExpenseAttachment>
{
    public void Configure(EntityTypeBuilder<ExpenseAttachment> builder)
    {
        builder.ToTable("ExpenseAttachments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.StoredFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(d => d.Egreso)
            .WithMany(p => p.Attachments)
            .HasForeignKey(d => d.EgresoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
