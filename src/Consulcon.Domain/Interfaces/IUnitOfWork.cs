using System.Threading.Tasks;

namespace Consulcon.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
