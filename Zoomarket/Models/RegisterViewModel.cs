using System.ComponentModel.DataAnnotations;

namespace Zoomarket.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress(ErrorMessage = "Некорректный Email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите ваше имя")]
        [Display(Name = "Ваше имя")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите номер телефона")]
        [StringLength(12, MinimumLength = 10, ErrorMessage = "Номер телефона должен содержать от 10 до 12 символов")]
        [RegularExpression(@"^\+?[0-9]{10,12}$", ErrorMessage = "Некорректный формат номера. Используйте только цифры (например, +79991234567)")]
        [Display(Name = "Номер телефона")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен быть от {2} до {1} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}