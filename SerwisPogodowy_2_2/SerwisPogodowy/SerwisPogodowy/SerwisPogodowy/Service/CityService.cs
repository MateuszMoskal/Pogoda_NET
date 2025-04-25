using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SerwisPogodowy.DataBase;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SerwisPogodowy.Service
{
    public class CityService : ICityService
    {
        private readonly DataBaseContext context;
        private ISessionService sessionService;
        private IRemoteWeatherRepository weatherRepository;
        private IDataBaseRepository dataBaseRepository;

        public CityService(DataBaseContext context, ISessionService sessionService, 
            IRemoteWeatherRepository weatherRepository,
            IDataBaseRepository dataBaseRepository)
        {
            this.context = context;
            this.sessionService = sessionService;
            this.weatherRepository = weatherRepository;
            this.dataBaseRepository = dataBaseRepository;

        }

        public async Task<List<City>> SelectCity(string cityName)
        {
            return await weatherRepository.GetCitiesAsync(cityName);
        }

        public async Task AddCityAsync(City city)
        {
            await dataBaseRepository.AddCityAsync(city, sessionService.User.Id);

            await GetWeatherForCityAsync(city);
        }

        public async Task<List<CityWheaterInformationVM>> ReadAllLocalizationsAsync()
        {
            int userId = sessionService.User.Id;
            List<CityWheaterInformationVM> listCityWheater = new List<CityWheaterInformationVM>();

            List<City> cities = await context.Cities.Where(city => city.UserId == userId).ToListAsync();

            foreach (City city in cities)
            {
                CityWheaterInformationVM cityWheater = new CityWheaterInformationVM();
                cityWheater.City = city;

                // Sprawdź, czy istnieją dane pogodowe dla miasta
                WeatherData? weatherData = await context.WeatherData
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

            WeatherData weatherData =await weatherRepository.GetWeatherForCityAsync(city);
            //tutaj do poprawy zapis jak nie ma a jak jest to ewentualny update
            context.WeatherData.Add(weatherData);
            await context.SaveChangesAsync();

            return weatherData;
        }

        public async Task<WheaterForecastVM> GetWeatherForWeekAsync(int cityId)
        {
            City city = await context.Cities.FindAsync(cityId);

            List<WeatherData> weatherList = await weatherRepository.GetWeatherForWeekAsync(city);

            WheaterForecastVM wheaterForecastVM = new WheaterForecastVM();

            foreach (var weatherData in weatherList)
            {

                WheaterVM wheater = new WheaterVM();
                wheater.Temperature = weatherData.Temperature;
                wheater.FeelsLike = weatherData.FeelsLike;
                wheater.Humidity = weatherData.Humidity;
                wheater.Pressure = weatherData.Pressure;
                wheater.WindSpeed = weatherData.WindSpeed;

                wheater.Description = weatherData.Description;
                wheater.Icon = weatherData.Icon;
                wheater.WindSpeed = weatherData.WindSpeed;
                wheater.Date = weatherData.Date;
                wheaterForecastVM.Forecast.Add(wheater);
            }

            // Zapisz dane pogodowe do bazy
            // context.WeatherData.Add(weatherData);
            // await context.SaveChangesAsync();

            return wheaterForecastVM;
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
        private DateTime LastUpdate()
        {
            DateTime now = DateTime.Now;

            int hour = now.Hour;
            hour = hour - hour % 3;

            return new DateTime(now.Year, now.Month, now.Day, hour, 0, 0);
        }

    }
}