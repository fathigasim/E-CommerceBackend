using EcommerceApplication.Features.Github.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.Queries.GithubProfile
{
    public record GetGithubUserProfileQuery(string username) : IRequest<List<RepoDto>>;
}
