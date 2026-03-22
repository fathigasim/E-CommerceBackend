using AutoMapper;
using EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using EcommerceApplication.Features.Products.Queries;
using EcommerceDomain.Interfaces;
using MediatR;


namespace Ecommerce.Application.Products.Queries
{
    public class GetPaginatedProductsQueryHandler :IRequestHandler<GetPaginatedProductsQuery,Result<PaginatedList<ProductDto>>>
    {                                           

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetPaginatedProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;

            _mapper = mapper;
        }


        public async Task<Result<PaginatedList<ProductDto>>> Handle(
    GetPaginatedProductsQuery request,
    CancellationToken cancellationToken)
        {

            var result = await _unitOfWork.Products.GetPagedAsync<ProductDto>(
          request.Page,
          request.PageSize,
         filter: p =>
        (string.IsNullOrEmpty(request.q) || p.Name.ToLower().Contains(request.q.ToLower())) &&
        (!request.categoryId.HasValue || p.CategoryId.Equals(request.categoryId)),
          orderBy: q => q.OrderByDescending(p => p.CreatedAt),
          cancellationToken: cancellationToken
      );
          

            return Result<PaginatedList<ProductDto>>.Success(
                new PaginatedList<ProductDto> {
                  Items=  result.Items,
                  PageNumber=  request.Page,
                  PageSize=  request.PageSize,
                 TotalCount= result.TotalCount});
        }
    }
}