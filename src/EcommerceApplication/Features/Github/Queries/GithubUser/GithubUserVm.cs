using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.Queries.GithubUser
{
    public class GithubUserVm
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public int RepoCount { get; set; }
    }
}
