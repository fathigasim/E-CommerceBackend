using EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Products.DTOs;
using EcommerceApplication.Interfaces;
using MediatR;

namespace EcommerceApplication.Features.Products.Queries
{

    public record GetPaginatedProductsQuery(
      string? q,
      Guid? categoryId,
      int Page,
      int PageSize = 6
  ) : IRequest<Result<PaginatedList<ProductDto>>>, ICacheableQuery
    {
        public string CacheKey =>
            $"ProductList-{q?.Trim().ToLower() ?? "all"}-" +
            $"{categoryId?.ToString() ?? "all"}-" +
            $"{Page}-{PageSize}";

        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }
}
