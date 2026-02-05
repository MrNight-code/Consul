using Consulcon.Domain.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Master;

public class CondominioMasterConfiguration : IEntityTypeConfiguration<CondominioMaster>
{
    public void Configure(EntityTypeBuilder<CondominioMaster> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.TenantId).IsUnique();
    }
}
