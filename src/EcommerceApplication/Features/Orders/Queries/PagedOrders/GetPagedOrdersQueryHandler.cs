using AutoMapper;
using EcommerceApplication.Common;
using EcommerceApplication.Common.EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Orders.DTOs;
using EcommerceDomain.Enums;
using EcommerceDomain.Interfaces;
using MediatR;


namespace EcommerceApplication.Features.Orders.Queries.PagedOrders
{
    public class GetPagedOrdersQueryHandler : IRequestHandler<GetPagedOrdersQuery, Result<PaginatedList<OrderDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPagedOrdersQueryHandler(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PaginatedList<OrderDto>>> Handle(GetPagedOrdersQuery request, CancellationToken cancellationToken)
      {
            DateTime searchDate;
            bool isDateSearch = DateTime.TryParse(request.q, out searchDate);
            Guid guidSearch;
            bool isGuidSearch = Guid.TryParse(request.q, out guidSearch);
            OrderStatus statusSearch;
            bool isStatusSearch = Enum.TryParse<OrderStatus>(request.q, true, out statusSearch);
            List<OrderStatus> statusMatches = new();
            if (!string.IsNullOrWhiteSpace(request.q))
            {
                statusMatches = Enum
                    .GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Where(e => e.ToString().Contains(request.q, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            var result = await _unitOfWork.Orders.GetPagedAsync<OrderDto>(
     request.Page,
     request.PageSize,
     cancellationToken: cancellationToken,
     filter: p =>
         string.IsNullOrEmpty(request.q)
         || p.OrderNumber.Contains(request.q.ToLowerInvariant())
         || (isGuidSearch && p.Id == guidSearch)
         || (
             isDateSearch &&
             p.OrderDate >= searchDate.Date &&
             p.OrderDate < searchDate.Date.AddDays(1)
         )
         ||   (statusMatches.Any() && statusMatches.Contains(p.Status))

         ,
     orderBy: p => p.OrderByDescending(o => o.OrderDate)
 );




            return Result<PaginatedList<OrderDto>>.Success(
                    new PaginatedList<OrderDto> {

                    Items = result.Items,
                  PageNumber = request.Page,
                  PageSize = request.PageSize,
                 TotalCount = result.TotalCount});
           

        }
            
        }
    }

