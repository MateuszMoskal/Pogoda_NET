using SerwisPogodowy.Models.ViewModels;

namespace SerwisPogodowy.Service
{
    public interface ICityService
    {
        Task<List<CityLocalication>> SelectCity(string cityName);
    }
}
