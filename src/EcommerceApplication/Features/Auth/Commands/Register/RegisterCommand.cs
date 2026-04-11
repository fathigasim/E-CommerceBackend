
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using MediatR;


namespace EcommerceApplication.Features.Auth.Commands.Register
{
    public record RegisterCommand(string Email, string Password, string ConfirmPassword, string FirstName, string LastName, string UserName) : IRequest<Result<string>>;
   

}
