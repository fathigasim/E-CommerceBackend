
using EcommerceApplication.Common.Settings;
using EcommerceDomain.Enums;
using EcommerceDomain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRTutorialApplication.Features.Payment.Commands.RefundPayment
{
    public class RefundPaymentHandler
     : IRequestHandler<RefundPaymentCommand, Result<RefundPaymentResponse>>
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public RefundPaymentHandler(
            IPaymentService paymentService,
           IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RefundPaymentResponse>> Handle(
            RefundPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment is null)
                return Result<RefundPaymentResponse>.Failure("Payment not found");

            if (payment.Status != PaymentStatus.Succeeded)
                return Result<RefundPaymentResponse>.Failure("Only succeeded payments can be refunded");

            var refundResult = await _paymentService.RefundPaymentAsync(
                payment.StripePaymentIntentId!,
                request.Amount,
                cancellationToken);

            payment.Status = PaymentStatus.Refunded;
            payment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Payments.UpdateAsync(payment, cancellationToken);
          var orderToUpdate=  await _unitOfWork.Orders.GetByIdAsync(payment.OrderId.Value, cancellationToken);
             orderToUpdate.Status= payment.Status switch
             {
                 PaymentStatus.Succeeded => OrderStatus.Processing,
                 PaymentStatus.Failed => OrderStatus.Cancelled,
                 PaymentStatus.Cancelled => OrderStatus.Cancelled,
                 PaymentStatus.Refunded => OrderStatus.Cancelled,
                 _ => orderToUpdate.Status
             };
             _unitOfWork.Orders.Update(orderToUpdate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<RefundPaymentResponse>.Success(
                new RefundPaymentResponse(
                    refundResult.RefundId,
                    refundResult.Status,
                    refundResult.AmountRefunded));
        }
    }
}
