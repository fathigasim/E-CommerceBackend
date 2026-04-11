using EcommerceApplication.Common;
using EcommerceApplication.Common.EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Orders.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Orders.Queries.PagedOrders
{
    public record GetPagedOrdersQuery(string? q,int Page, int PageSize = 6) :IRequest<Result<PaginatedList<OrderDto>>>;
    
}
