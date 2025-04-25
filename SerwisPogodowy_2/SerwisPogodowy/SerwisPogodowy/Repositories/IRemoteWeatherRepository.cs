using SerwisPogodowy.Models;

namespace SerwisPogodowy.Repositories
{
    public interface IRemoteWeatherRepository
    {
        Task<List<City>> GetCitiesAsync(string cityName);

        Task<WeatherData> GetWeatherForCityAsync(City city);

        Task<List<WeatherData>> GetWeatherForWeekAsync(City city);
    }
}
