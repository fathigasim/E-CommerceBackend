using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using MediaRTutorialApplication.Interfaces;
using MediaRTutorialDomain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResponse>
    {
  
        private readonly IIdentityService _identityService;
        public LogoutCommandHandler(
        
            IIdentityService identityService)
        {
       
            _identityService = identityService;
        }

        public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            //var user = await _userManager.FindByIdAsync(request.UserId);
            var userDto = await _identityService.FindByIdAsync(request.UserId);
            if (userDto != null)
            {
                userDto.RefreshToken = null;
                userDto.RefreshTokenExpiry = null;
                await _identityService.UpdateAsync(userDto);
            }

            await _identityService.SignOutAsync();

            return new LogoutResponse
            {
                Succeeded = true,
                Message = "Logged out successfully"
            };
        }
    }
}
