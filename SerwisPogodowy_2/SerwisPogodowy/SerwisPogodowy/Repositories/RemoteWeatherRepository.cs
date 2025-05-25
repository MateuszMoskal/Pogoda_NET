using Newtonsoft.Json.Linq;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using System.Collections.Generic;

namespace SerwisPogodowy.Repositories
{
    public class RemoteWeatherRepository : IRemoteWeatherRepository
    {
        private static readonly HttpClient client = new HttpClient();
        private const string API_CODE = "731d5f5250da097e6f57df58e3feea33";

        public async Task<List<City>> GetCitiesAsync(string cityName)
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
                    LastUpdated = LastUpdate(),
                    Date = LastUpdate()
                };

                return weatherData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania danych pogodowych: {ex.Message}");
                return null;
            }

        }

        public async Task<List<WeatherData>> GetWeatherForWeekAsync(City city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?lat={city.Latitude}&lon={city.Longitude}&appid={API_CODE}";

            WheaterForecastVM wheaterForecastVM = new WheaterForecastVM();

            var response = await client.GetStringAsync(url);
            JObject weatherJson = JObject.Parse(response);

            List<WeatherData> weatherList = new List<WeatherData>();

            foreach (var wheatherTimestamp in weatherJson["list"])
            {
                WeatherData weatherData = new WeatherData()
                {
                    Temperature = ReadTemperature(wheatherTimestamp["main"]["temp"]),
                    FeelsLike = ReadTemperature(wheatherTimestamp["main"]["feels_like"]),
                    Humidity = wheatherTimestamp["main"]["humidity"]?.ToObject<int>() ?? 0,
                    Pressure = wheatherTimestamp["main"]["pressure"]?.ToObject<int>() ?? 0,
                    WindSpeed = wheatherTimestamp["wind"]["speed"]?.ToObject<double>() ?? 0,
                    Description = wheatherTimestamp["weather"][0]["description"]?.ToString(),
                    Icon = wheatherTimestamp["weather"][0]["icon"]?.ToString(),
                    CityId = city.Id,
                    LastUpdated = LastUpdate(),
                    Date = DateTime.Parse(wheatherTimestamp["dt_txt"].ToString())
                };
                weatherList.Add(weatherData);
            }
            return weatherList;
        }


        private DateTime LastUpdate()
        {
            DateTime now = DateTime.Now;

            int hour = now.Hour;
            hour = hour - hour % 3;

            return new DateTime(now.Year, now.Month, now.Day, hour, 0, 0);
        }

        private double ReadTemperature(JToken? jtokenTemperature)
        {
            double temperature = jtokenTemperature?.ToObject<double>() ?? 0;
            return temperature - 273.15;

        }
    }
}
