

using EcommerceApplication.Features.Github.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Interfaces
{
  
        public interface IHttpClientService
        {
            Task<string> GetDataAsync(CancellationToken cancellationToken);
            Task<GithubUserDto?> GetGithubUserAsync(string username, CancellationToken cancellationToken);
            
    }
    
}
