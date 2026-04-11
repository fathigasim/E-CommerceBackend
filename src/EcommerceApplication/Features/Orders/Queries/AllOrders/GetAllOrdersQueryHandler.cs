using AutoMapper;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Orders.DTOs;
using EcommerceDomain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Orders.Queries.AllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<IReadOnlyList<OrderDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyList<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Orders.GetOrdersAsync(cancellationToken);
            var orderDtos = _mapper.Map<IReadOnlyList<OrderDto>>(orders);
            return Result<IReadOnlyList<OrderDto>>.Success(orderDtos);
        }
    }
}