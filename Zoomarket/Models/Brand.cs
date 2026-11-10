using System.ComponentModel.DataAnnotations;

namespace Zoomarket.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название бренда")]
        [Display(Name = "Название бренда")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Ссылка на логотип")]
        public string? LogoUrl { get; set; }

        [Display(Name = "Описание бренда")]
        public string? Description { get; set; }
    }
}