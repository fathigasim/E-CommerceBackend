using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Products.DTOs;
using EcommerceApplication.Interfaces;
using MediatR;

namespace EcommerceApplication.Features.Products.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>, ICacheableQuery
    {
        public string CacheKey => $"GetProduct-{Id}";

        public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    }

}
