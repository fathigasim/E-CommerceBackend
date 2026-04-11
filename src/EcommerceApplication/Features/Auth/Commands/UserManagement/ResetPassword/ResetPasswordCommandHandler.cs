using EcommerceApplication.Common.Settings;
using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using MediaRTutorialApplication.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Auth.Commands.UserManagement.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;
        public ResetPasswordCommandHandler(IIdentityService identityService,
            ILogger<ResetPasswordCommandHandler> logger
            )
        {
            _identityService = identityService;
            _logger = logger;
        }
        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return
                    Result<string>.Failure("no such user");
            }
            if (request.NewPasswordConfirm != request.NewPassword)
            {
                return Result<string>.Failure("Password do not match");
            }
            // ✅ DECODE THE BASE64 TOKEN
            string decodedToken;
            //try
            //{
            decodedToken = DecodeBase64Token(request.Token);
            _logger.LogInformation("Decoded token successfully");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Failed to decode token");
            //    return Result<string>.Failure("Invalid token format");
            //}

            var result = await _identityService.ResetPasswordAsync(user, decodedToken, request.NewPassword);
           return result;
        }



        private static string DecodeBase64Token(string base64Token)
        {
            // Handle URL-safe Base64 (replace - with + and _ with /)
            string base64 = base64Token
                .Replace('-', '+')
                .Replace('_', '/');

            // Add padding if necessary
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
