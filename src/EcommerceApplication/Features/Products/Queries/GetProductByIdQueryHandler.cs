using AutoMapper;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Products.DTOs;
using EcommerceApplication.Features.Products.Queries;
using EcommerceDomain.Interfaces;
using MediatR;

namespace MediaRTutorialApplication.Features.Products.Queries
{
    public class GetProductByIdQueryHandler
      : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ProductDto>> Handle(
            GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products
                .GetProductWithCategoryAsync(request.Id, cancellationToken);

            if (product is null)
                return Result<ProductDto>.Failure("Product not found.");

            var dto = _mapper.Map<ProductDto>(product);
            return Result<ProductDto>.Success(dto);
        }
    }
}
