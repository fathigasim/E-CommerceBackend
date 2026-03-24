using AutoMapper;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Products.DTOs;
using EcommerceDomain.Interfaces;
using MediatR;


namespace EcommerceApplication.Features.Products.Queries
{
    public class GetAllProductsQueryHandler
     : IRequestHandler<GetAllProductsQuery, Result<IReadOnlyList<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<ProductDto>>> Handle(
            GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
            if (products == null)
            {
                var productDtos = _mapper.Map<IReadOnlyList<ProductDto>>(products);
                return Result<IReadOnlyList<ProductDto>>.Success(productDtos);
            }
            return Result<IReadOnlyList<ProductDto>>.Failure("No products found sorry!!");
        }
    }
}
