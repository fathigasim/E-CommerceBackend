using AutoMapper;
using Azure.Core;
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using EcommerceApplication.Features.Auth.Commands.Login;
using EcommerceApplication.Features.Auth.Dtos;
using EcommerceApplication.Interfaces;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;


namespace EcommerceInfrastructure.Identity
{
    public class IdentityService :IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAuthenticationService authenticationService,
            IOptions<JwtSettings> jwtSettings,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;
            _jwtSettings = jwtSettings.Value;
            _mapper = mapper;
        }
      
        public async Task<Result<string>> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Result<string>.Failure("Email is already registered.");
            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<string>.Failure(errors);
            }

            await _userManager.AddToRoleAsync(user, "User");
            
             var userDto= _mapper.Map<UserDto>(user);
            // var token = await GenerateJwtToken(user);
            var (accessToken, refreshToken) = await _authenticationService.GenerateTokens(userDto);
            //return Result<LoginResponse>.Success(
            // new   LoginResponse
            //{
            //    Succeeded = true,
            //    AccessToken = accessToken,
            //    RefreshToken = refreshToken,
            //    Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            //    User = new UserDto
            //    {
            //        Id = user.Id,
            //        Email = user.Email,
            //        UserName = user.UserName,
            //        FirstName = user.FirstName,
            //        LastName = user.LastName,
            //        Roles = roles.ToList()
            //    }
            //}
            
            //);
            return Result<string>.Success("User registered successfully.");
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginDto dto)
        {

            var user = await _userManager.FindByEmailAsync(dto.Email);
            var userDto= _mapper.Map<UserDto>(user);
            if (user == null)
                return Result<LoginResponse>.Failure("Invalid email or password.");
            if(!user.IsActive)
                return Result<LoginResponse>.Failure("User account is inactive. Please contact support.");
            var result = await _signInManager
                .CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Result<LoginResponse>.Failure("Invalid email or password.");

            //var token = await GenerateJwtToken(user);
                                   var roles=  await _userManager.GetRolesAsync(user);
            var (accessToken, refreshToken) = await _authenticationService.GenerateTokens(userDto);
            return Result<LoginResponse>.Success(new LoginResponse() { 
                
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                 User = new UserDto
                 {
                     Id = user.Id,
                     Email = user.Email,
                     UserName = user.UserName,
                     FirstName = user.FirstName,
                     LastName = user.LastName,
                     Roles = roles.ToList()
                 }
            }
            );
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
        };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserDto> FindByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateAsync(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user != null)
            {
                _mapper.Map(userDto, user);
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> GeneratePasswordResetTokenAsync(UserDto user)
        {
            return  await _userManager.GeneratePasswordResetTokenAsync(_mapper.Map<ApplicationUser>(user));
        }

        public async Task<UserDto> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<Result<string>> ResetPasswordAsync(UserDto user, string token, string newPassword)
        {
             var result = await _userManager.ResetPasswordAsync(_mapper.Map<ApplicationUser>(user), token, newPassword);
            if (!result.Succeeded)
            {
                return Result<string>.Failure($"Password change failed ");
            }
         //   _logger.LogInformation("Password changed for user {User}", user);
            return Result<string>.Success("Password changed successfully.");
        }

        public async Task<Result<string>> DeleteAsync(string userId) {          
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return Result<string>.Success("User deleted successfully.");
            }
            return Result<string>.Failure("User not found.");
        }
    }

}
