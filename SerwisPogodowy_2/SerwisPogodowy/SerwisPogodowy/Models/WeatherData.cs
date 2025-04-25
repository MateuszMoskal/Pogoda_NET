using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SerwisPogodowy.Models
{
    [Table("WeatherData")]
    public class WeatherData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        // Relacja z miastem
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        // Timestamp ostatniej aktualizacji
        public DateTime LastUpdated { get; set; }

        public DateTime Date { get; set; }
    }
}