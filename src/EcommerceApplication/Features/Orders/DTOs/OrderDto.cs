


namespace EcommerceApplication.Features.Orders.DTOs
{
    public record OrderDto(
    Guid Id,
    string UserId,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    List<OrderItemDto> Items
);

}
