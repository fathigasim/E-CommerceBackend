using AutoMapper;
using EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Payment.DTOs;
using EcommerceDomain.Enums;
using EcommerceDomain.Interfaces;
using MediaRTutorialApplication.DTOs;
using MediaRTutorialApplication.Features.Payment.Queries.GetPaymentById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Payment.Queries.PaymentList
{
    public class PaymentListQueryHandler : IRequestHandler<PaymentListQuery ,Result<PaginatedList<PaymentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PaymentListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<PaginatedList<PaymentDto>>> Handle(PaymentListQuery request , CancellationToken cancellationToken)
        {


            var pagedPayments = await _unitOfWork.Payments.GetPagedAsync<PaymentDto>(
            
      pageNumber: request.Page,
      pageSize: request.PageSize,
      
      orderBy: q => q.OrderByDescending(p => p.CreatedAt), // optional
      filter: p => p.Status == PaymentStatus.Succeeded,     // optional
      cancellationToken: cancellationToken
  );

            return Result<PaginatedList<PaymentDto>>.Success(
                 new PaginatedList<PaymentDto>
                 {
                     Items = pagedPayments.Items,
                     PageNumber = request.Page,
                     PageSize = request.PageSize,
                     TotalCount = pagedPayments.TotalCount
                 });
        
                


        }
    }
}
