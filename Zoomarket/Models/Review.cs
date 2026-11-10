using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Zoomarket.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите ваше имя")]
        [Display(Name = "Ваше имя")]
        public string AuthorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите текст отзыва")]
        [Display(Name = "Текст отзыва")]
        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;

        [Display(Name = "Оценка")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public int Rating { get; set; } = 5;

        [Display(Name = "Дата отзыва")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

      
        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
    }
}