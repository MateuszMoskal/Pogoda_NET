using System.ComponentModel.DataAnnotations;

namespace SerwisPogodowy.Models.ViewModels
{
    public class CitySearchVM
    {
        [Required(ErrorMessage = "Nazwa miasta jest wymagana")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwa miasta musi mieć od 2 do 50 znaków")]
        public string CityName { get; set; } = string.Empty;

        public List<City>? Cities { get; set; } = null;
    }
}