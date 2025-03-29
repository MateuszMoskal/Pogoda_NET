using System.ComponentModel.DataAnnotations;

namespace SerwisPogodowy.Models.ViewModels
{
    public class UserRegisterVM
    {
        [Required]
        public string Email { get; set; }
       
        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordConfirm { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
