using EcommerceApplication.Features.Github.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Interfaces
{
    public interface IGitHubService
    {
        Task<GitHubUser> GetUserAsync(string username);
        Task<List<RepoDto>> GetUserRepositoriesAsync(string username,CancellationToken cancellationToken);
        
    }
}
