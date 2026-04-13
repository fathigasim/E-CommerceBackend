
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Orders.Commands;
using EcommerceDomain.Entities;
using EcommerceDomain.Enums;
using EcommerceDomain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRTutorialApplication.Features.Payment.Commands.CreatePaymentIntent
{
    public class CreatePaymentIntentHandler
     : IRequestHandler<CreatePaymentIntentCommand, Result<CreatePaymentIntentResponse>>
    {
        private readonly IPaymentService _paymentService;
       private readonly IBasketContextAccessor _basketContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreatePaymentIntentHandler> _logger;
        private readonly IMediator _mediator;
        

        public CreatePaymentIntentHandler(
            IPaymentService paymentService,
            IBasketContextAccessor basketContextAccessor,
            IUnitOfWork unitOfWork,
            ILogger<CreatePaymentIntentHandler> logger,
             IMediator mediator)
        {
            _paymentService = paymentService;
            _basketContextAccessor = basketContextAccessor;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<Result<CreatePaymentIntentResponse>> Handle(
            CreatePaymentIntentCommand request,
            CancellationToken cancellationToken)
        {
            var transaction =  _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
               
                var basket = await _unitOfWork.Baskets.GetByIdAsync(_basketContextAccessor.GetBasketId(), includeItems: true);
                var basketItems = basket?.BasketItems ?? new List<BasketItem>();
                var orderId = await _mediator.Send(new CreateOrderCommand());
                var metadata = new Dictionary<string, string>
        {
            { "userId", request.UserId },
            { "orderId", orderId.Data.ToString() },
            { "productNames", string.Join(", ", basketItems.Select(p => p.Product.Name)) },
            { "productIds", string.Join(", ", basketItems.Select(p => p.ProductId.ToString())) },
            { "itemCount", basketItems.Count.ToString() }
        };
                var intentResult = await _paymentService.CreatePaymentIntentAsync(
                    request.Amount,
                    request.Currency,
                    request.CustomerEmail,
                    metadata,
                    cancellationToken);

                // 2. Persist payment record in our database
                var payment = new EcommerceDomain.Entities.Payment
                {
                 //   Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    OrderId =orderId.Data,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    StripePaymentIntentId = intentResult.PaymentIntentId,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await  _unitOfWork.Payments.AddAsync(payment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _basketContextAccessor.ClearBasketId();
                // 3. Return client secret to frontend
                return Result<CreatePaymentIntentResponse>.Success(
                    new CreatePaymentIntentResponse(
                        payment.Id,
                        intentResult.ClientSecret,
                        intentResult.PaymentIntentId));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Failed to create payment intent");
                return Result<CreatePaymentIntentResponse>.Failure(ex.Message);
            }
        }
    }
}
