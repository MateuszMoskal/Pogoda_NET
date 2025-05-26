using System.ComponentModel.DataAnnotations;

namespace SerwisPogodowy.Models.ViewModels
{
    public class UserRegisterVM
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format emaila")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane")]
        [Compare("Password", ErrorMessage = "Hasła muszą być identyczne")]
        public string PasswordConfirm { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
    }
}