using Microsoft.EntityFrameworkCore;
using SerwisPogodowy.DataBase;
using SerwisPogodowy.Models;
using SerwisPogodowy.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SerwisPogodowy.Repositories
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private readonly DataBaseContext context;

        public DataBaseRepository(DataBaseContext context)
        {
            this.context = context;
        }

        public User? ReadUser(string email, string hashPassword)
        {
            User? userFromBase = context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashPassword);
            return userFromBase;
        }

        public bool UserExiest(string email)
        {
            return context.Users.Any(u => u.Email == email);
        }

        public User CreateUser(string email, string hashPassword)
        {
            User user = new User();
            user.Email = email;
            user.Password = hashPassword;
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public async Task<City?> ReadCityAsync(int cityId)
        {
            return await context.Cities.FindAsync(cityId);
        }

        public async Task AddCityAsync(City city, int userId)
        {
            User user = await context.Users.FindAsync(userId);
            city.User = user;
            city.UserId = user.Id; // POPRAWKA: city.UserId = user.Id zamiast city.Id = user.Id

            context.Cities.Add(city);
            await context.SaveChangesAsync();
        }



        public async Task DeleteCityAsync(int cityId)
        {
            
            City? city = await context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);

            if (city != null)
            {
                // Usuń również powiązane dane pogodowe
                var weatherData = await context.WeatherData
                    .Where(w => w.CityId == cityId)
                    .ToListAsync();

                context.WeatherData.RemoveRange(weatherData);
                context.Cities.Remove(city);
                await context.SaveChangesAsync();
            }
        }

        public async Task<WeatherData?> RetriveWheaterDataAsync(int cityId, DateTime date)
        {
            return context.WeatherData.Where(w => w.CityId == cityId && w.Date == date).FirstOrDefault();
            
        }
        public async Task<List<WeatherData>> RetriveWheaterDataAsync(int cityId, DateTime begin, DateTime end)
        {
            return await context.WeatherData.Where(w => w.CityId == cityId && begin <= w.Date && w.Date < end).ToListAsync();

        }

        public async Task AddWheaterAsync(WeatherData weather)
        {
            context.WeatherData.Add(weather);
            await context.SaveChangesAsync();
        }

        public async Task UpdateWeatherAsync(WeatherData weather)
        {
            var existingWeather = await context.WeatherData.FindAsync(weather.Id);

            if (existingWeather != null)
            {
                context.Entry(existingWeather).CurrentValues.SetValues(weather);
                await context.SaveChangesAsync();
            }
        }

    }
}
