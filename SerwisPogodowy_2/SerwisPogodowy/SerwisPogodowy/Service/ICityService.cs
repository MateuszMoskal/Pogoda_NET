using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;

namespace SerwisPogodowy.Service
{
    public interface ICityService
    {
        Task<List<City>> SelectCity(string cityName);
        Task AddCityAsync(City city);
        Task<List<CityWheaterInformationVM>> ReadAllLocalizationsAsync();

    }
}
