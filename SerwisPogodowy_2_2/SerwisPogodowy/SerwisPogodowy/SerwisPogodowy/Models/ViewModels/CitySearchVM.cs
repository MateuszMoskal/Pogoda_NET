namespace SerwisPogodowy.Models.ViewModels
{
    public class CitySearchVM
    {
        public string CityName { get; set; } = string.Empty;

        public List<City>? Cities { get; set; } = null;
    }
}
