using EcommerceApplication.Common.Settings;
using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using EcommerceDomain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;
        private readonly IEmailSender _emailSender;
        
        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager,
            ILogger<ResetPasswordCommandHandler> logger, IEmailSender emailSender)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
        }
        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.email);
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

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);



            // TODO: Send email with reset token
             await _emailSender.SendEmailAsync(user.Email, token, $"<p>Copy link for password reset: <strong>{token}</strong></p>", cancellationToken);

            _logger.LogInformation("Password reset requested for user {Email}", request.email);

            return Result<string>.Success("If the email exists, a password reset link has been sent.");


        }
    }

}
