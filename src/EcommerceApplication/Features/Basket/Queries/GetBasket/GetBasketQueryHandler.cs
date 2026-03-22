
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Basket.DTOs;
using EcommerceDomain.Interfaces;
using MediatR;


namespace MediaRTutorialApplication.Features.Basket.Queries.GetBasket
{
    public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, Result<BasketDto>>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IBasketContextAccessor _basketContextAccessor;

        public GetBasketQueryHandler(
            IBasketRepository basketRepository,
            IBasketContextAccessor basketContextAccessor)
        {
            _basketRepository = basketRepository;
            _basketContextAccessor = basketContextAccessor;
        }

        public async Task<Result<BasketDto>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basketId = _basketContextAccessor.GetBasketId();
            if (string.IsNullOrEmpty(basketId))
                return Result<BasketDto>.Success(BasketDto.Empty);

            var basket = await _basketRepository.GetByIdAsync(basketId, includeItems: true);
            if (basket == null)
                return Result<BasketDto>.Success(BasketDto.Empty);

            var dto = new BasketDto(
                basket.BasketId,
                basket.BasketItems.Select(i => new BasketItemDto(
                    i.BasketItemId,
                    i.ProductId,
                    i.Product.Name,
                    i.Quantity,
                    i.Product.Price,
                    i.Product.ImageUrl
                )).ToList(),
                basket.GetTotal(),
                basket.GetItemCount()
            );

            return Result<BasketDto>.Success(dto);
        }
    }
}
