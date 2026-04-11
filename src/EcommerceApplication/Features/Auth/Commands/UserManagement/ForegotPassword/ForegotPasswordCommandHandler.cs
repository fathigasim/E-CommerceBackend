using EcommerceApplication.Common.Settings;
using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using EcommerceDomain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Auth.Commands.UserManagement.ResetPassword
{
    public class ForegotPasswordCommandHandler : IRequestHandler<ForegotPasswordCommand, Result<string>>
    {
        
        private readonly IIdentityService _identityService;
        private readonly ILogger<ForegotPasswordCommandHandler> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        public ForegotPasswordCommandHandler(IIdentityService identityService,
            ILogger<ForegotPasswordCommandHandler> logger, IEmailSender emailSender, IConfiguration config)
        {
            _identityService = identityService;
            _logger = logger;
            _emailSender = emailSender;
            _config = config;   
        }
        public async Task<Result<string>> Handle(ForegotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user doesn't exist
                return Result<string>.Success("If the email exists, a password reset link has been sent.");
                //{
                //    Success = true,
                //    Message = "If the email exists, a password reset link has been sent.",
                //    Data = true
                //};
            }

            var token = await _identityService.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetLink = $"{_config["FrontendUrl:FrontendUrl"]}/reset-password?email={user.Email}&token={encodedToken}";
            var subject = "Reset Your Password";

            var message = $@"
    <h2>Password Reset Request</h2>
    <p>Hello {user.UserName},</p>
    <p>You requested to reset your password.</p>
    <p>Click the button below to reset your password:</p>
    
    <p>
        <a href='{resetLink}' 
           style='padding:10px 20px; background-color:#007bff; color:white; text-decoration:none; border-radius:5px;'>
           Reset Password
        </a>
    </p>

    <p>If you did not request this, please ignore this email.</p>
    <p>This link will expire soon for security reasons.</p>
";

            //await _emailSender.SendEmailAsync(
            //    user.Email,
            //    subject,
            //    encodedToken,
            //    cancellationToken
            //);
            // TODO: Send email with reset token
            await _emailSender.SendEmailAsync(user.Email, subject, message, cancellationToken);

            _logger.LogInformation("Password reset requested for user {Email}", request.Email);

            return Result<string>.Success("If the email exists, a password reset link has been sent.");


        }
    }

}
