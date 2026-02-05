using Consulcon.Domain.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consulcon.Infrastructure.Persistence.Entities.Master;

public class UsuarioCondominioConfiguration : IEntityTypeConfiguration<UsuarioCondominio>
{
    public void Configure(EntityTypeBuilder<UsuarioCondominio> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasOne(d => d.Usuario)
            .WithMany(p => p.Condominios)
            .HasForeignKey(d => d.UsuarioId);

        builder.HasOne(d => d.Condominio)
            .WithMany(p => p.Usuarios)
            .HasForeignKey(d => d.CondominioId);
    }
}
