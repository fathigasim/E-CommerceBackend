using EcommerceApplication.Features.Orders.DTOs;


namespace EcommerceApplication.Features.Payment.DTOs
{
    //public class PaymentDto
    //{
    //    public string UserId { get; set; } = string.Empty;
    //    public decimal Amount { get; set; }
    //    public string Currency { get; set; } = "usd";
    //    public string? StripePaymentIntentId { get; set; }
    //    public string? StripeCustomerId { get; set; }
    //    public PaymentStatus Status { get; set; }
    //    public Guid? OrderId { get; set; }
    //    public Order? Order { get; set; }
    //}
    public record PaymentDto(
    Guid Id,
    string UserId,
    decimal Amount,
    string Currency,
    string Status,
    string? StripePaymentIntentId,
     Guid? OrderId,
     OrderDto? Order,
    DateTime CreatedAt
);
}
