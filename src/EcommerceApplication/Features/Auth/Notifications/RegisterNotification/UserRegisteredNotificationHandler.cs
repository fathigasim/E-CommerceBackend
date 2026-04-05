using EcommerceDomain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Auth.Notifications.RegisterNotification
{
    public class UserRegisteredNotificationHandler : INotificationHandler<UserRegisteredNotification>
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UserRegisteredNotificationHandler> _logger;

        public UserRegisteredNotificationHandler(IEmailSender emailSender, ILogger<UserRegisteredNotificationHandler> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Handle(UserRegisteredNotification notification, CancellationToken ct)
        {
            try
            {
                await _emailSender.SendEmailAsync(
                    notification.Email,
                    "Welcome!",
                    $"Hi {notification.Name}, thanks for joining us!",ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", notification.Email);
                // In a production app, you might queue this for a retry
            }
        }
    }
}
