using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Пароль и пароль подтверждения не совпадают.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
