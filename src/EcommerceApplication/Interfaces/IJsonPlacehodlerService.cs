using EcommerceApplication.Common.EcommerceApplication.Common;
using EcommerceApplication.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Interfaces
{
    public interface IJsonPlacehodlerService
    {
        Task<PaginatedList<JsonUserDto>> GetPostsAsync(string? q, int PageNumber, int PageSize, CancellationToken cancellationToken);
    }
}
