using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Github.DTOs
{
    public class GithubUserDto
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Public_Repos { get; set; }
        public int Followers { get; set; }
    }
}
