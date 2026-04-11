using EcommerceApplication.Common.Settings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Auth.Commands.UserManagement.DeleteUser
{
    public record DeleteUserCommand(string userId) : IRequest<Result<string>>;
    
}
