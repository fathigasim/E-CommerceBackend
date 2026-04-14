using EcommerceApplication.Common.EcommerceApplication.Common;
using EcommerceApplication.DTOs;
using EcommerceApplication.Exceptions;
using EcommerceApplication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceInfrastructure.Services
{
    public class JsonPlacehodlerService : IJsonPlacehodlerService
    {
        readonly HttpClient _httpClient;
        public JsonPlacehodlerService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<PaginatedList<JsonUserDto>> GetPostsAsync(string? q, int PageNumber, int PageSize, CancellationToken cancellationToken)
        {
            // var url = $"posts?_page={PageNumber}&_limit={PageSize}";
            var response = await _httpClient.GetAsync("posts", cancellationToken);
            response.EnsureSuccessStatusCode();
            var allPosts = await response.Content.ReadFromJsonAsync<List<JsonUserDto>>(cancellationToken);
            if (allPosts is null || !allPosts.Any())
            {
                throw new NotFoundException("Sorry no data found");
            }

            var query = allPosts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p => p.Title.Contains(q.Trim(), StringComparison.OrdinalIgnoreCase) ||
                 p.Body.Contains(q.Trim(), StringComparison.OrdinalIgnoreCase)
               );
            }
            var count = query.Count();
            // If no items match the search, throw your exception here
            if (count == 0) throw new NotFoundException("No matching results found");
            return new PaginatedList<JsonUserDto>
                (query.Skip((PageNumber - 1) * PageSize)
                .Take(PageSize).ToList(), PageNumber, PageSize, count);

        }
    }
}
