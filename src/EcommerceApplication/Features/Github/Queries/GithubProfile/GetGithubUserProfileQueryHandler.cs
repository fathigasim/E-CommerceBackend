using EcommerceApplication.Features.Github.DTOs;
using EcommerceApplication.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.Queries.GithubProfile
{
    public class GetGithubUserProfileQueryHandler : IRequestHandler<GetGithubUserProfileQuery, List<RepoDto>>
    {
        private readonly IGitHubService _gitHubService;
        public GetGithubUserProfileQueryHandler(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }
        public Task<List<RepoDto>> Handle(GetGithubUserProfileQuery request, CancellationToken cancellationToken)
        {
           var result= _gitHubService.GetUserRepositoriesAsync(request.username,cancellationToken);
                
            return result;
        }
    }
}
