using AutoMapper;
using EcommerceDomain.Entities;
using EcommerceDomain.Interfaces;
using EcommerceInfrastructure.Persistance;
using EcommerceInfrastructure.Repository;
using Microsoft.EntityFrameworkCore;


namespace EcommerceInfrastructure.Repository
{
    public class ProductRepository : PagedRepository<Product>,IProductRepository
         
    {
        public ProductRepository(AppDbContext context, IMapper mapper) : base(context, mapper) { }

        public async Task<IReadOnlyList<Product>> GetProductsByIdAsync(
         IEnumerable<Guid> prodIds, CancellationToken cancellationToken = default)
        {
            return await _context.Products

                .Where(p => prodIds.Contains(p.Id))
                //.AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(
            Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetProductWithCategoryAsync(
            Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        }
    }

}
