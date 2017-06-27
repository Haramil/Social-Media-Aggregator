using System.ComponentModel.DataAnnotations;

namespace AggregatorServer.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Заполните логин")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Заполните пароль")]
        [Display(Name = "Пароль")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
