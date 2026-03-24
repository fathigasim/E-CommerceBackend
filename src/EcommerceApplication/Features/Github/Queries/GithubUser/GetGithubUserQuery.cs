using EcommerceApplication.Common.Settings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.Queries.GethubUser
{
    public record GetGithubUserQuery : IRequest<string>;
}
