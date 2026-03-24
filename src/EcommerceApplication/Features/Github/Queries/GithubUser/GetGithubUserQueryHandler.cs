using EcommerceApplication.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.Queries.GethubUser
{
    public class GetGithubUserQueryHandler : IRequestHandler<GetGithubUserQuery, string>
    {
        private readonly IHttpClientService _httpClientService;

        public GetGithubUserQueryHandler(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<string> Handle(GetGithubUserQuery request, CancellationToken cancellationToken)
        {
            return await _httpClientService.GetDataAsync(cancellationToken);
        }
    }
}
