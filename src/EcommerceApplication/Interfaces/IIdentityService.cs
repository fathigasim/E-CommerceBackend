using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using EcommerceApplication.Features.Auth.Commands.Login;
using EcommerceApplication.Features.Auth.Dtos;
using EcommerceDomain.Entities;
using MediatR;


namespace EcommerceApplication.Interfaces
{

    public interface IIdentityService
    {
        Task<UserDto> FindByIdAsync(string userId);
        Task<string> GeneratePasswordResetTokenAsync(UserDto user);
        Task<Result<string>> ResetPasswordAsync(UserDto user, string token, string newPassword);
        Task<UserDto> FindByEmailAsync(string email);
        Task UpdateAsync(UserDto userDto);
        Task SignOutAsync();
        Task<Result<string>> DeleteAsync(string userId);
        Task<Result<string>> RegisterAsync(RegisterDto dto);
        Task<Result<LoginResponse>> LoginAsync(LoginDto dto);
        
    }
}