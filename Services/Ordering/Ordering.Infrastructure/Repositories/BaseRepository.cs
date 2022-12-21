using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistance;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Ordering.Infrastructure.Repositories
{
    public class BaseRepository<T> : IAsyncRepository<T> where T : EntityBase
    {
        protected readonly OrderContext _orderContext;
        private DbSet<T> _query;

        public BaseRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
            _query = _orderContext.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _query.AddAsync(entity);
            await _orderContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _orderContext.Entry(entity).State = EntityState.Modified;
            await _orderContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _query.Remove(entity);
            await _orderContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _query.Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        {
            var query = _query.AsQueryable();

            if (disableTracking) query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString))
                query = query.Include(includeString);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
        {
            var query = _query.AsQueryable();

            if (disableTracking) query = query.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query, (current,
                    include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _query.FindAsync(id);
        }
    }
}
