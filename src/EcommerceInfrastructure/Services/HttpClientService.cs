
using EcommerceApplication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EcommerceApplication.Features.Github.DTOs;
namespace EcommerceInfrastructure.Services
{
    public class HttpClientService :IHttpClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetDataAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("EcommerceApp");
            client.Timeout = TimeSpan.FromSeconds(30);
            
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            
            return await client.GetStringAsync("https://api.github.com/users/fathigasim", cancellationToken);
        }
        public async Task<GithubUserDto> GetGithubUserAsync(string username, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("GitHub");

            var response = await client.GetAsync(
                $"https://api.github.com/users/{username}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"GitHub API Error: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GithubUserDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result!;
        }



    }
}
