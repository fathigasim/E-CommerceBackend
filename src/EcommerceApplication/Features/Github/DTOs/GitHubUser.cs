using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.DTOs
{
  
    public record GitHubUser(string Login, string Name, string AvatarUrl);
}
