using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SerwisPogodowy.Models
{
    [Table("Cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kod kraju jest wymagany")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Kod kraju musi mieć 2-3 znaki")]
        [RegularExpression(@"^[A-Z]{2,3}$", ErrorMessage = "Kod kraju musi składać się z wielkich liter")]
        public string CantryCode { get; set; } = string.Empty; // Poprawiona nazwa

        [StringLength(50, ErrorMessage = "Nazwa województwa może mieć maksymalnie 50 znaków")]
        public string? Voivodeship { get; set; }

        [Required(ErrorMessage = "Nazwa miasta jest wymagana")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Nazwa miasta musi mieć od 1 do 100 znaków")]
        public string Name { get; set; } = string.Empty;

        [Range(-180, 180, ErrorMessage = "Długość geograficzna musi być z zakresu -180 do 180")]
        public double Longitude { get; set; }

        [Range(-90, 90, ErrorMessage = "Szerokość geograficzna musi być z zakresu -90 do 90")]
        public double Latitude { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }

        public virtual ICollection<WeatherData> WeatherData { get; set; } = new List<WeatherData>();

        public DateTime? LastUpdatedCurrent { get; set; }
        public DateTime? LastUpdatedWeekly { get; set; }
    }
}