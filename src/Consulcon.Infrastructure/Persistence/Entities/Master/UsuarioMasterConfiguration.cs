using Consulcon.Domain.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Master;

public class UsuarioMasterConfiguration : IEntityTypeConfiguration<UsuarioMaster>
{
    public void Configure(EntityTypeBuilder<UsuarioMaster> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Username).IsUnique();
    }
}
