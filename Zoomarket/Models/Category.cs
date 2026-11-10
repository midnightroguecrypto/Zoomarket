using System.ComponentModel.DataAnnotations;

namespace Zoomarket.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название категории")]
        [Display(Name = "Название категории")]
        public string Name { get; set; } = string.Empty;

        public List<Product> Products { get; set; } = new();
    }
}