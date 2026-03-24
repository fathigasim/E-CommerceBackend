using AutoMapper;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Github.Queries.GithubUser;
using EcommerceApplication.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.Queries.GithubUsers
{
    public class GetGithubUsersQueryHandler : IRequestHandler<GetGithubUsersQuery, Result<GithubUserVm>>
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IMapper _mapper;

        public GetGithubUsersQueryHandler(
            IHttpClientService httpClientService,
            IMapper mapper)
        {
            _httpClientService = httpClientService;
            _mapper = mapper;
        }

        public async Task<Result<GithubUserVm>> Handle(GetGithubUsersQuery request, CancellationToken cancellationToken)
        {
            var dto = await _httpClientService.GetGithubUserAsync(request.Username, cancellationToken);
              var dtoResult= _mapper.Map<GithubUserVm>(dto);
            if (string.IsNullOrEmpty(dtoResult.DisplayName))  return Result<GithubUserVm>.Failure("No user repo found") ;
            return Result<GithubUserVm>.Success(dtoResult);
        }
    }
}
