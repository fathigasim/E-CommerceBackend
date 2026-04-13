using EcommerceDomain.Entities;
using MediaRTutorialDomain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceDomain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        //Task<PaginatedList<TResult>> GetPagedAsync<TResult>(
        //    int pageNumber,
        //    int pageSize,
        //    Expression<Func<T, bool>>? filter = null,
        //    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        //    CancellationToken cancellationToken = default
        //);

        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ItemExistAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
    }
}
