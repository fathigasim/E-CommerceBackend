using AutoMapper;
using EcommerceApplication.Common.Localization;
using EcommerceApplication.Common.Localization.Resources;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using EcommerceDomain.Enums;
using EcommerceDomain.Interfaces;
using MediaRTutorialApplication.Interfaces;


using MediatR;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Orders.Commands
{
    public class CreateOrderCommandHandler: IRequestHandler<CreateOrderCommand,Result<Guid>>

    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly IBasketContextAccessor _basketContextAccessor;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IOrderNumberService _orderNumberService;
        public CreateOrderCommandHandler(
            IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser,
            IBasketContextAccessor basketContextAccessor,
            IStringLocalizer<SharedResource> localizer,ILogger<CreateOrderCommandHandler> logger,
            IOrderNumberService orderNumberService
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
            _basketContextAccessor = basketContextAccessor;
            _logger = logger;
            _localizer= localizer;
            _orderNumberService = orderNumberService;
        }

        public async Task<Result<Guid>> Handle(
    CreateOrderCommand request,
    CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated)
                return Result<Guid>.Failure("User is not authenticated.");

            var basketId = _basketContextAccessor.GetBasketId();
            if (string.IsNullOrEmpty(basketId))
                return Result<Guid>.Failure("No active basket found for the user.");

            try
            {
                //  Pass cancellationToken consistently
                _logger.LogInformation("Starting DB Operation: GetBasketItems");
                var basketItems = await _unitOfWork.Baskets
                    .GetBasketItemsAsync(basketId, CancellationToken.None);
                _logger.LogInformation("Finished DB Operation: GetBasketItems");
                if (!basketItems.Any())
                    return Result<Guid>.Failure("Basket is empty.");
                var basket = await _unitOfWork.Baskets.GetByIdAsync(basketId);

                // 1. Create the parent order first
                var orderNumber = await _orderNumberService.GenerateOrderNumberAsync();
                var order = new Order
                {
                    UserId = _currentUser.UserId!,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = basketItems.Sum(b => b.Quantity * b.Product.Price),
                    OrderNumber = orderNumber
                    // Id is already Guid.NewGuid() thanks to BaseEntity!
                };

                // 2. Map the items now that 'order' exists
                order.Items = basketItems.Select(bi => new OrderItem
                {
                    OrderId = order.Id, // This is now safe to use!
                    ProductId = bi.ProductId,
                    Quantity = bi.Quantity,
                    UnitPrice = bi.Product.Price
                }).ToList();
                // Update Payment using PaymentIntentId
                //if (!string.IsNullOrEmpty(request.PaymentIntentId))
                //{
                //    var payment = await _unitOfWork.Payments
                //        .GetByStripePaymentIntentIdAsync(request.PaymentIntentId, cancellationToken);

                //    if (payment != null)
                //    {
                //        payment.OrderId = order.Id;
                //        payment.Status = PaymentStatus.Succeeded;
                //        payment.UpdatedAt = DateTime.UtcNow;

                //        await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
                //    }
                //}
                await _unitOfWork.Orders.AddAsync(order, cancellationToken);
                //  var updatePaymentResult = await _unitOfWork.Payments.Update(); want to update payment orderId
                await _unitOfWork.Baskets.DeleteAsync(basket);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

             

                return Result<Guid>.Success(order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating order for user {UserId}: {Message}",
                    _currentUser.UserId, ex.Message);
            //    _localizer["CreateSuccess", _localizer["Product"]] 
                return Result<Guid>.Failure("An error occurred while creating the order.");
            }
        }
    }
}