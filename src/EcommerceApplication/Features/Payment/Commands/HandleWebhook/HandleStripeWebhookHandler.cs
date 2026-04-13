
using EcommerceApplication.Common.Settings;
using EcommerceDomain.Enums;
using EcommerceDomain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRTutorialApplication.Features.Payment.Commands.HandleWebhook
{
    public class HandleStripeWebhookHandler
    : IRequestHandler<HandleStripeWebhookCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HandleStripeWebhookHandler> _logger;

        public HandleStripeWebhookHandler(
           IUnitOfWork unitOfWork,
            ILogger<HandleStripeWebhookHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(
            HandleStripeWebhookCommand request,
            CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments
                .GetByStripePaymentIntentIdAsync(request.PaymentIntentId, cancellationToken);

            if (payment is null)
            {
                _logger.LogWarning("Payment not found for intent: {Id}", request.PaymentIntentId);
                return Result<bool>.Failure("Payment not found");
            }

            payment.Status = request.EventType switch
            {
                "payment_intent.succeeded" => PaymentStatus.Succeeded,
                "payment_intent.payment_failed" => PaymentStatus.Failed,
                "payment_intent.canceled" => PaymentStatus.Cancelled,
                "charge.refunded" => PaymentStatus.Refunded,
                _ => payment.Status
            };

            payment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
            //if(payment.OrderId is null)
            //{
            //    _logger.LogWarning("Payment {Id} has no associated order", payment.Id);
            //    return Result<bool>.Failure("Payment has no associated order");
            //}
              var orderToUpdate= await _unitOfWork.Orders.GetByIdAsync(payment.OrderId.Value, cancellationToken);
            if (payment.OrderId != null)
            {
                orderToUpdate.Status = payment.Status switch
                {
                    PaymentStatus.Succeeded => OrderStatus.Processing,
                    PaymentStatus.Failed => OrderStatus.Cancelled,
                    PaymentStatus.Cancelled => OrderStatus.Cancelled,

                    _ => payment.Order.Status
                };
            }
           // _unitOfWork.Orders.Update(orderToUpdate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation(
              "Update {OrderId} updated to ", payment.OrderId);
            _logger.LogInformation(
                "Payment {Id} status updated to {Status}", payment.Id, payment.Status);

            return Result<bool>.Success(true);
        }
    }
}
