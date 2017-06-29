using System.ComponentModel.DataAnnotations;

namespace AggregatorServer.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Заполните логин")]
        [Display(Name = "Логин")]
        [MinLength(3, ErrorMessage = "Минимальная длина логина - 3 символов")]
        [MaxLength(255, ErrorMessage = "Максимальная длина логина - 255 символов")]
        [RegularExpression("^[a-zA-Z1-9_]+$", ErrorMessage = "Логин должен содержать только латинские буквы, цифры от 1 до 9 и символ нижнего подчёркивания")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Заполните пароль")]
        [Display(Name = "Пароль")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
        [MaxLength(30, ErrorMessage = "Максимальная длина пароля - 30 символов")]
        [RegularExpression("^[a-zA-Z1-9_]+$", ErrorMessage = "Пароль должен содержать только латинские буквы, цифры от 1 до 9 и символ нижнего подчёркивания")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
