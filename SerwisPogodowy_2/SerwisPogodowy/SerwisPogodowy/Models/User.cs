using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SerwisPogodowy.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format emaila")]
        [StringLength(100, ErrorMessage = "Email może mieć maksymalnie 100 znaków")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(255, ErrorMessage = "Hasło może mieć maksymalnie 255 znaków")]
        public string Password { get; set; } = string.Empty;

        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}