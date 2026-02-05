using System.Threading.Tasks;
using Consulcon.Domain.Interfaces;
using Consulcon.Domain;

namespace Consulcon.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork(ConsulconDbContext context) : IUnitOfWork
    {
        private readonly ConsulconDbContext _context = context;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
