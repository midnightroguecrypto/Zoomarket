using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zoomarket.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название товара")]
        [Display(Name = "Название товара")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Краткое описание")]
        public string? ShortDescription { get; set; }

        [Required(ErrorMessage = "Укажите цену")]
        [Range(1, 100000000, ErrorMessage = "Цена должна быть больше 0")] 
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Цена (₽)")]
        public decimal Price { get; set; }

        [Display(Name = "Старая цена (если есть скидка)")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 100000000, ErrorMessage = "Старая цена слишком велика")] 
        public decimal? OldPrice { get; set; }

        [Display(Name = "Ссылка на картинку")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Бренд")]
        public string? Brand { get; set; } 

        [Display(Name = "Дата добавления")]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Категория")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<Promotion> Promotions { get; set; } = new();
    }
}