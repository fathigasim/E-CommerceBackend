using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Products.Notifications
{
    public record ProductUpdatedNotification(Guid ProductId) : INotification;
}
