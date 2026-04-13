


using AutoMapper;
using EcommerceDomain.Entities;
using EcommerceDomain.Interfaces;
using EcommerceInfrastructure.Repository;
using EcommerceInfrastructure.Persistance;
using Microsoft.EntityFrameworkCore;


namespace EcommerceInfrastructure.Repository
{
    public class BasketRepository : Repository<Basket>, IBasketRepository
    {


        public BasketRepository(AppDbContext context) : base(context) { }


        public async Task<Basket> GetByIdAsync(string basketId, bool includeItems = false)
        {
            var query = _context.Basket.AsQueryable();

            if (includeItems)
            {
                query = query
                    .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.Product);
            }

            var basket = await query.FirstOrDefaultAsync(b => b.BasketId == basketId);

            if (basket is null)
                throw new KeyNotFoundException($"Basket {basketId} not found");

            return basket;
        }

        public async Task<Basket> CreateAsync(Basket basket)
        {
            await _context.Basket.AddAsync(basket);
            return basket;
        }

        public Task UpdateAsync(Basket basket)
        {
            _context.Basket.Update(basket);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Basket basket)
        {
            _context.Basket.Remove(basket );
            return Task.CompletedTask;
        }

        public async Task<BasketItem?> GetBasketItemAsync(string basketId, Guid productId)
        {
            return await _context.BasketItems
                .FirstOrDefaultAsync(i => i.BasketId == basketId && i.ProductId == productId);
        }

        public async Task AddItemAsync(BasketItem item)
        {
            await _context.BasketItems.AddAsync(item);
        }

        public Task RemoveItemAsync(BasketItem item)
        {
            _context.BasketItems.Remove(item);
            return Task.CompletedTask;
        }
        public Task RemoveBasketAsync(Basket basket)
        {
            _context.Basket.Remove(basket);
            return Task.CompletedTask;
        }

        public async Task<List<BasketItem>> GetBasketItemsAsync(string basketId,CancellationToken cancellationToken)
        {
            return await _context.BasketItems
                .Include(bi => bi.Product)
                .Where(bi => bi.BasketId == basketId)
             
                  .ToListAsync(cancellationToken);
        }

        

     
    }
}
