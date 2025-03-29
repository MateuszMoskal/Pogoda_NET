using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SerwisPogodowy.Models
{
    [Table("Cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public string CantryCode { get; set; }

        public string CityName { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }
    }
}
