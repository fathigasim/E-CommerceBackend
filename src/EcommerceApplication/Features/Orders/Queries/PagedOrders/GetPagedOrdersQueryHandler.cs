using AutoMapper;
using EcommerceApplication.Common;
using EcommerceApplication.Common.EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Orders.DTOs;
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
            var result = await _unitOfWork.Orders.GetPagedAsync<OrderDto>(
                request.Page,request.PageSize,cancellationToken:cancellationToken,filter:
                 p=> string.IsNullOrEmpty(request.q)||p.Id.ToString().Contains(request.q),
                  orderBy: p=>p.OrderByDescending(o=>o.OrderDate)
                );
            if (result.Items.Any())
            {
               


                return Result<PaginatedList<OrderDto>>.Success(
                    new PaginatedList<OrderDto> {

                    Items = result.Items,
                  PageNumber = request.Page,
                  PageSize = request.PageSize,
                 TotalCount = result.TotalCount});
            }

            return Result<PaginatedList<OrderDto>>.Failure("No orders to fetch");
        }
            
        }
    }

