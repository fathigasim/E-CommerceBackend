

using EcommerceDomain.Entities;
using MediaRTutorialDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceDomain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>,IPagedRepository<Order>
    {

        Task<IReadOnlyList<Order>> GetOrdersAsync(CancellationToken cancellationToken = default);
     
        Task<IReadOnlyList<Order>> GetOrdersByUserAsync(
            string userId, CancellationToken cancellationToken = default);
        Task<Order?> GetOrderWithItemsAsync(
            Guid orderId, CancellationToken cancellationToken = default);
    }

}
