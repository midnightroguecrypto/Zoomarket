using Microsoft.AspNetCore.Identity;

namespace Zoomarket.Models
{
    public class CartItem
    {
        public int Id { get; set; } 

        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public int Quantity { get; set; } = 1;
    }
}