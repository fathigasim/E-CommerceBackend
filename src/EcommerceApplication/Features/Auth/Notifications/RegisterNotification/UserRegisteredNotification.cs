using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Auth.Notifications.RegisterNotification
{
    public record UserRegisteredNotification(string Email, string Name) : INotification;
    
}
