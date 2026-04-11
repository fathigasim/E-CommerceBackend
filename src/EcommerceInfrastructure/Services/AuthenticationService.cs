using AutoMapper;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.Features.Auth.Dtos;
using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using EcommerceInfrastructure.Identity;
using MediaRTutorialApplication.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceInfrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityService _identityService;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
             IMapper mapper
            )
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _mapper = mapper;
        }

        public async Task<string> GenerateJwtToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var identityUser = await _userManager.FindByEmailAsync(user.Email);

            var roles = await _userManager.GetRolesAsync(identityUser);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identityUser.Id),
                new Claim(ClaimTypes.Email, identityUser?.Email),
                new Claim(ClaimTypes.Name, identityUser?.UserName),
                new Claim("FirstName", identityUser.FirstName ?? ""),
                new Claim("LastName", identityUser.LastName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokens(UserDto user)
        {
            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            identityUser.RefreshToken = refreshToken;
            identityUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            await _userManager.UpdateAsync(identityUser);

            return (accessToken, refreshToken);
        }

        public async Task<UserDto> ValidateRefreshToken(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
