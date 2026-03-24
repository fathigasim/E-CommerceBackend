using EcommerceApplication.Features.Github.DTOs;
using EcommerceApplication.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
namespace EcommerceInfrastructure.Services
{
    public class GitHubService :IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GitHubService> _logger;
        private readonly IMemoryCache _cache;
        public GitHubService(HttpClient httpClient, ILogger<GitHubService> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<GitHubUser> GetUserAsync(string username)
        {
            var user = await _httpClient.GetFromJsonAsync<GitHubUser>(
                $"/users/{username}"
            );
            return user;
        }

        public async Task<List<RepoDto>> GetUserRepositoriesAsync(string username,CancellationToken cancellationToken)
        {
            _logger.LogInformation($"print httpclient base address{0}",_httpClient.BaseAddress);
            var cacheKey = $"github_repos_{username}";

            if (_cache.TryGetValue(cacheKey, out List<RepoDto> cachedRepos))
            {
                return cachedRepos;
            }

            var repos = await _httpClient.GetFromJsonAsync<List<RepoDto>>(
                $"users/{username}/repos",cancellationToken
            );

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };

            _cache.Set(cacheKey, repos, cacheOptions);

            return repos ?? new List<RepoDto>();
        }
    }

}
