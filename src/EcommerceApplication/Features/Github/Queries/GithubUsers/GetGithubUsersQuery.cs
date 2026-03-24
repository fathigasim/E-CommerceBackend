using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Github.Queries.GithubUser;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.Queries.GithubUsers
{
    public record GetGithubUsersQuery(string Username) : IRequest<Result<GithubUserVm>>;
}
