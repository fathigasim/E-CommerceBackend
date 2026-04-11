using EcommerceApplication.Features.Auth.Dtos;
using EcommerceDomain.Entities;
using MediaRTutorialDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> GenerateJwtToken(UserDto user);
        string GenerateRefreshToken();
        Task<(string AccessToken, string RefreshToken)> GenerateTokens(UserDto user);
        Task<UserDto> ValidateRefreshToken(string userId, string refreshToken);
    }
}
