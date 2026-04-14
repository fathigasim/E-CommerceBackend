using EcommerceApplication.Common.EcommerceApplication.Common;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using EcommerceApplication.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.JsonPlaceholder.Queries
{
    public class JsonPlaceholderQueryHandler : IRequestHandler<JsonPlaceholderQuery, Result<PaginatedList<JsonUserDto>>>
    {
        private readonly IJsonPlacehodlerService _jsonPlacehodlerService;

        public JsonPlaceholderQueryHandler(IJsonPlacehodlerService jsonPlacehodlerService)
        {
            _jsonPlacehodlerService = jsonPlacehodlerService;
        }

        public async Task<Result<PaginatedList<JsonUserDto>>> Handle(JsonPlaceholderQuery request, CancellationToken cancellationToken)
        {
            var posts = await _jsonPlacehodlerService.GetPostsAsync(request.q, request.PageNumber, request.PageSize, cancellationToken);
            return Result<PaginatedList<JsonUserDto>>.Success(posts);
        }
    }
}
