using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SerwisPogodowy.DataBase;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SerwisPogodowy.Service
{
    public class CityService : ICityService
    {
        private readonly DataBaseContext context;
        private static readonly HttpClient client = new HttpClient();
        private const string API_CODE = "xxx";
        private ISessionService sessionService;

        public CityService(DataBaseContext context, ISessionService sessionService)
        {
            this.context = context;
            this.sessionService = sessionService;
        }

        public async Task<List<City>> SelectCity(string cityName)
        {
            string url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=5&appid={API_CODE}";

            try
            {
                var response = await client.GetStringAsync(url);
                JArray jsonArray = JArray.Parse(response);
                List<City> cities = new List<City>();

                if (jsonArray == null || !jsonArray.Any())
                {
                    return cities;
                }

                foreach (var locationJson in jsonArray)
                {
                    City city = new City();
                    city.Name = locationJson["name"]?.ToString();
                    city.CantryCode = locationJson["country"]?.ToString();
                    city.Voivodeship = locationJson["state"]?.ToString();
                    city.Latitude = locationJson["lat"]?.ToObject<double>() ?? 0;
                    city.Longitude = locationJson["lon"]?.ToObject<double>() ?? 0;
                    cities.Add(city);
                }

                return cities;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wyszukiwania miasta: {ex.Message}");
                return new List<City>();
            }
        }

        public async Task AddCityAsync(City city)
        {
            User user = await context.Users.FindAsync(sessionService.User.Id);
            city.User = user;
            city.UserId = user.Id; // POPRAWKA: city.UserId = user.Id zamiast city.Id = user.Id

            context.Cities.Add(city);
            await context.SaveChangesAsync();

            // Po dodaniu miasta, pobierz od razu dane pogodowe
            await GetWeatherForCityAsync(city);
        }

        public async Task<List<CityWheaterInformationVM>> ReadAllLocalizationsAsync()
        {
            int userId = sessionService.User.Id;
            List<CityWheaterInformationVM> listCityWheater = new List<CityWheaterInformationVM>();

            // Pobierz miasta użytkownika
            List<City> cities = await context.Cities.Where(city => city.UserId == userId).ToListAsync();

            foreach (City city in cities)
            {
                CityWheaterInformationVM cityWheater = new CityWheaterInformationVM();
                cityWheater.City = city;

                // Sprawdź, czy istnieją dane pogodowe dla miasta
                WeatherData weatherData = await context.WeatherData
                    .Where(w => w.CityId == city.Id)
                    .OrderByDescending(w => w.LastUpdated)
                    .FirstOrDefaultAsync();

                // Jeśli dane nie istnieją lub są starsze niż 30 minut, pobierz nowe
                if (weatherData == null || (DateTime.Now - weatherData.LastUpdated).TotalMinutes > 30)
                {
                    weatherData = await GetWeatherForCityAsync(city);
                }

                // Jeśli mamy dane pogodowe, uzupełnij ViewModel
                if (weatherData != null)
                {
                    cityWheater.Temperature = weatherData.Temperature;
                    cityWheater.FeelsLike = weatherData.FeelsLike;
                    cityWheater.Humidity = weatherData.Humidity;
                    cityWheater.Pressure = weatherData.Pressure;
                    cityWheater.WindSpeed = weatherData.WindSpeed;
                    cityWheater.Description = weatherData.Description;
                    cityWheater.Icon = weatherData.Icon;
                }

                listCityWheater.Add(cityWheater);
            }

            return listCityWheater;
        }

        // NOWA METODA: Pobieranie danych pogodowych dla miasta
        public async Task<WeatherData> GetWeatherForCityAsync(City city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={city.Latitude}&lon={city.Longitude}&units=metric&appid={API_CODE}&lang=pl";

            try
            {
                var response = await client.GetStringAsync(url);
                JObject weatherJson = JObject.Parse(response);

                WeatherData weatherData = new WeatherData
                {
                    Temperature = weatherJson["main"]["temp"]?.ToObject<double>() ?? 0,
                    FeelsLike = weatherJson["main"]["feels_like"]?.ToObject<double>() ?? 0,
                    Humidity = weatherJson["main"]["humidity"]?.ToObject<int>() ?? 0,
                    Pressure = weatherJson["main"]["pressure"]?.ToObject<int>() ?? 0,
                    WindSpeed = weatherJson["wind"]["speed"]?.ToObject<double>() ?? 0,
                    Description = weatherJson["weather"][0]["description"]?.ToString(),
                    Icon = weatherJson["weather"][0]["icon"]?.ToString(),
                    CityId = city.Id,
                    LastUpdated = DateTime.Now
                };

                // Zapisz dane pogodowe do bazy
                context.WeatherData.Add(weatherData);
                await context.SaveChangesAsync();

                return weatherData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania danych pogodowych: {ex.Message}");
                return null;
            }
        }

        // NOWA METODA: Aktualizacja danych pogodowych dla wszystkich miast
        public async Task UpdateWeatherDataForAllCitiesAsync()
        {
            int userId = sessionService.User.Id;
            List<City> cities = await context.Cities
                .Where(city => city.UserId == userId)
                .ToListAsync();

            foreach (var city in cities)
            {
                await GetWeatherForCityAsync(city);
            }
        }

        // NOWA METODA: Usunięcie miasta i jego danych pogodowych
        public async Task DeleteCityAsync(int cityId)
        {
            int userId = sessionService.User.Id;
            City city = await context.Cities
                .FirstOrDefaultAsync(c => c.Id == cityId && c.UserId == userId);

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
    }
}