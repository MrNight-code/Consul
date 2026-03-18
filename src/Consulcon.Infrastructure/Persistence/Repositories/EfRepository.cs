using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Consulcon.Domain.Interfaces;
using Consulcon.Domain; // Required for ConsulconDbContext

namespace Consulcon.Infrastructure.Persistence.Repositories
{
    public class EfRepository<T>(ConsulconDbContext context) : IRepository<T> where T : class
    {
        protected readonly ConsulconDbContext _context = context;
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _context.Set<T>();
             if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return await query.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<T>> GetPagedAsync(ISpecification<T> spec, int pageNumber, int pageSize)
        {
            // 1. Obtener la cuenta total SIN paginación
            var countQuery = SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec);
            var totalRecords = await countQuery.CountAsync();

            // 2. Obtener los items CON paginación (si la especificación la tiene)
            var query = SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec);

            // 3. Si la especificación NO define paginación, la aplicamos aquí basándonos en los parámetros
            if (!spec.IsPagingEnabled)
            {
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }

            var items = await query.ToListAsync();

            return new PagedResult<T>(items, pageNumber, pageSize, totalRecords);
        }
    }
}
