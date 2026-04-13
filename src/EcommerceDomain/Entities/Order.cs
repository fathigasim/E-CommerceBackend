using EcommerceDomain.Common;
using EcommerceDomain.Entities;
using EcommerceDomain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceDomain.Entities
{ 

   public class Order : BaseEntity, IAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
        public string OrderNumber { get; set; }= default!;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    
    public ICollection<Payment>? Payments { get; set; } = new List<Payment>();  
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public void CalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}

}

