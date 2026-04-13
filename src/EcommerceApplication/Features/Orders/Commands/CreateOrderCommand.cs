using EcommerceApplication.Common.Settings;

using MediatR;


namespace EcommerceApplication.Features.Orders.Commands
{
    

    // CreateOrderCommand.cs
    public class CreateOrderCommand : IRequest<Result<Guid>>
    {
        //public string? PaymentIntentId { get; set; }
    }
}
