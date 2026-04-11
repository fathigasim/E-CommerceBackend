
using EcommerceApplication.Common.Settings;
using EcommerceApplication.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<Result<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //public record LoginCommand(string Email, string Password)
    //: IRequest<Result<AuthResponseDto>>;
}
