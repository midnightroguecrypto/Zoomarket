using System.ComponentModel.DataAnnotations;

namespace Zoomarket.Models
{
    public class Promotion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название акции")]
        [Display(Name = "Название акции")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Описание акции")]
        public string? Description { get; set; }

        [Display(Name = "Ссылка на баннер (картинку)")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Размер скидки (текст, например '15%')")]
        public string? DiscountText { get; set; }

        [Display(Name = "Дата окончания")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        public List<Product> Products { get; set; } = new();

        [Display(Name = "Процент скидки (%)")]
        [Range(0, 100, ErrorMessage = "Скидка должна быть от 0 до 100%")]
        public int DiscountPercent { get; set; } = 0;
    }
}