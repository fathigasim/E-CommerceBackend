


namespace EcommerceApplication.Features.Orders.DTOs
{
    public record OrderDto(
    Guid Id,
    string UserId,
 string OrderNumber ,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    List<OrderItemDto> Items
);

}
