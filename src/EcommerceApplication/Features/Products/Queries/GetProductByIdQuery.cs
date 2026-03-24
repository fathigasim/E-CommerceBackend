using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Products.DTOs;
using MediatR;

namespace EcommerceApplication.Features.Products.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

}
