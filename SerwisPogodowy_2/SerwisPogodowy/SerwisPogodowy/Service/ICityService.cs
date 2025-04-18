using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
namespace SerwisPogodowy.Service
{
    public interface ICityService
    {
        Task<List<City>> SelectCity(string cityName);
        Task AddCityAsync(City city);
        Task<List<CityWheaterInformationVM>> ReadAllLocalizationsAsync();

        // Dodaj te metody:
        Task<WeatherData> GetWeatherForCityAsync(City city);
        Task UpdateWeatherDataForAllCitiesAsync();
        Task DeleteCityAsync(int cityId);
    }
}