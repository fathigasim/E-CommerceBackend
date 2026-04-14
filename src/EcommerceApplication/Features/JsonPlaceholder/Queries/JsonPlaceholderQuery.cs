using EcommerceApplication.Common.EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.JsonPlaceholder.Queries
{
    public record JsonPlaceholderQuery(string? q, int PageNumber, int PageSize) : IRequest<Result<PaginatedList<JsonUserDto>>>;
}
