using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;  // Dodaj ten import

namespace SerwisPogodowy.Models
{
    [Table("Cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CantryCode { get; set; }
        public string? Voivodeship { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        // Dodaj relację z danymi pogodowymi
        public virtual ICollection<WeatherData> WeatherData { get; set; }

        public DateTime? LastUpdatedCurrent { get; set; }
        public DateTime? LastUpdatedWeekly { get; set; }
    }
}