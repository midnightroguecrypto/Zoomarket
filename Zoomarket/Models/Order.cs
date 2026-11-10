using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Zoomarket.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "В обработке";
        public decimal TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }

        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        [Display(Name = "Причина отмены")]
        public string? CancelReason { get; set; }
    }
}