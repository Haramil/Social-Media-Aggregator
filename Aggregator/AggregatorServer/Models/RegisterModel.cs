using System.ComponentModel.DataAnnotations;

namespace AggregatorServer.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Заполните логин")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Заполните пароль")]
        [Display(Name = "Пароль")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Заполните пароль")]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
