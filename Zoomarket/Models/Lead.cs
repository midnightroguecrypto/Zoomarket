using System.ComponentModel.DataAnnotations;

namespace Zoomarket.Models
{
    public class Lead
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Сообщение")]
        public string? Message { get; set; }

        [Display(Name = "Дата заявки")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        [Display(Name = "Статус")]
        public string Status { get; set; } = "Новая"; 
    }
}