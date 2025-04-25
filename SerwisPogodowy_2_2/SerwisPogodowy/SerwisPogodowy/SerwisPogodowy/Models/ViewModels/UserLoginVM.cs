using System.ComponentModel.DataAnnotations;

namespace SerwisPogodowy.Models.ViewModels
{
    public class UserLoginVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

    }
}
