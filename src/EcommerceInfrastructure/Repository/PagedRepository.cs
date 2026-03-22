using AutoMapper;
using AutoMapper.QueryableExtensions;
using EcommerceDomain.Entities;
using EcommerceDomain.Interfaces;
using EcommerceInfrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace EcommerceInfrastructure.Repository
{
    public class PagedRepository<T> : Repository<T>, IPagedRepository<T> where T : class
    {
        private readonly IMapper _mapper;

        public PagedRepository(AppDbContext context, IMapper mapper) : base(context)
        {
           
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PaginatedList<TResult>> GetPagedAsync<TResult>(
  
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (filter != null)
                query = query.Where(filter);
         
            var totalCount = await query.CountAsync(cancellationToken);

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<TResult>(items, totalCount, pageNumber, pageSize);
        }
    }
}
