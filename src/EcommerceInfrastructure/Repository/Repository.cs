
using EcommerceDomain.Interfaces;
using EcommerceInfrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceInfrastructure.Repository
{
    //public class Repository<T> : IRepository<T> where T : class
    //{
    //    protected readonly AppDbContext _context;
    //    protected readonly DbSet<T> _dbSet;
    //    private readonly IMapper _mapper;

    //    public Repository(AppDbContext context, IMapper mapper)
    //    {
    //        _context = context ?? throw new ArgumentNullException(nameof(context));
    //        _dbSet = context.Set<T>();
    //        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    //    }

    //    // 🔥 CLEAN PAGINATION + PROJECTION
    //    public async Task<PaginatedList<TResult>> GetPagedAsync<TResult>(
    //        int pageNumber,
    //        int pageSize,
    //        Expression<Func<T, bool>>? filter = null,
    //        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    //        CancellationToken cancellationToken = default
    //    )
    //    {
    //        IQueryable<T> query = _dbSet.AsNoTracking();

    //        if (filter != null)
    //            query = query.Where(filter);

    //        var totalCount = await query.CountAsync(cancellationToken);

    //        if (orderBy != null)
    //            query = orderBy(query);

    //        var items = await query
    //            .ProjectTo<TResult>(_mapper.ConfigurationProvider)
    //            .Skip((pageNumber - 1) * pageSize)
    //            .Take(pageSize)
    //            .ToListAsync(cancellationToken);

    //        return new PaginatedList<TResult>(items, totalCount, pageNumber, pageSize);
    //    }

    //    // 🔹 BASIC CRUD

    //    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    //    {
    //        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    //    }

    //    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    //    {
    //        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    //    }

    //    public async Task<IReadOnlyList<T>> FindAsync(
    //        Expression<Func<T, bool>> predicate,
    //        CancellationToken cancellationToken = default)
    //    {
    //        return await _dbSet
    //            .Where(predicate)
    //            .AsNoTracking()
    //            .ToListAsync(cancellationToken);
    //    }

    //    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    //    {
    //        await _dbSet.AddAsync(entity, cancellationToken);
    //        return entity;
    //    }

    //    public void Update(T entity)
    //    {
    //        _dbSet.Attach(entity);
    //        _context.Entry(entity).State = EntityState.Modified;
    //    }

    //    public void Delete(T entity)
    //    {
    //        _dbSet.Remove(entity);
    //    }

    //    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    //    {
    //        return await _dbSet.AnyAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
    //    }
    //}
    public class Repository<T> : IRepository<T>  where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
        }

        public async Task<bool> ItemExistAsync(


         Expression<Func<T, bool>> filter,

         CancellationToken cancellationToken = default)
        {
            //IQueryable<T> query = _dbSet.Where(filter).AsNoTracking();
            var exist = await _dbSet.AsNoTracking().AnyAsync(filter, cancellationToken);
            return exist;
        }
        //   Rule to Remember

        //LINQ filters must always return bool
    }
}