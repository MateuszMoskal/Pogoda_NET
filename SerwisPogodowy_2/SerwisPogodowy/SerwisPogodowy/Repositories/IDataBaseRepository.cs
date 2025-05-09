using SerwisPogodowy.Models;

namespace SerwisPogodowy.Repositories
{
    public interface IDataBaseRepository
    {
        User? ReadUser(string email, string hashPassword);
        bool UserExiest(string email);

        User CreateUser(string email, string hashPassword);

        Task AddCityAsync(City city, int userId);

        Task<City?> ReadCityAsync(int cityId);

        Task DeleteCityAsync(int cityId);

        Task<WeatherData?> RetriveWheaterDataAsync(int cityId, DateTime date);

        Task<List<WeatherData>> RetriveWheaterDataAsync(int cityId, DateTime begin, DateTime end);

        Task AddWheaterAsync(WeatherData weather);


        Task UpdateWeatherAsync(WeatherData weather);


    }

}
