using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Orders.Queries.AllOrders
{
    public record GetAllOrdersQuery : IRequest<Result<IReadOnlyList<OrderDto>>>;
}
    
