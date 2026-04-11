using EcommerceApplication.Common;
using EcommerceApplication.Common.EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Payment.DTOs;
using MediaRTutorialApplication.Features.Payment.Queries.GetPaymentById;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Payment.Queries.PaymentList
{
    public record PaymentListQuery(int Page, int PageSize) : IRequest<Result<PaginatedList<PaymentDto>>>;
    
}
