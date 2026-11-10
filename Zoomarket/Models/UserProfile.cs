using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Zoomarket.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string FullName { get; set; } = string.Empty;

        // Связь с пользователем (1:1)
        public string UserId { get; set; } = string.Empty;
        public virtual IdentityUser? User { get; set; }
    }
}