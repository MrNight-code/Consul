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

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_context.Database.CurrentTransaction != null)
                await _context.Database.CurrentTransaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_context.Database.CurrentTransaction != null)
                await _context.Database.CurrentTransaction.RollbackAsync();
        }
    }
}
