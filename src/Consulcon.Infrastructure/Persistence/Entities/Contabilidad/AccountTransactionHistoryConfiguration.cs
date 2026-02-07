
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Contabilidad
{
    public class AccountTransactionHistoryConfiguration : IEntityTypeConfiguration<global::Consulcon.Domain.Entities.Contabilidad.AccountTransactionHistory>
    {
        public void Configure(EntityTypeBuilder<global::Consulcon.Domain.Entities.Contabilidad.AccountTransactionHistory> builder)
        {
            builder.ToTable("AccountTransactionHistory");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(e => e.Date)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.ReferenceId)
                .HasMaxLength(100);

            builder.HasOne(d => d.Account)
                   .WithMany() 
                   .HasForeignKey(d => d.AccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Expense)
                   .WithMany()
                   .HasForeignKey(d => d.ExpenseId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
