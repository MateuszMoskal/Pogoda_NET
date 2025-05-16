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

                WeatherData weatherData = await GetWeatherForCityAsync(city);
             
                cityWheater.Temperature = weatherData.Temperature;
                cityWheater.FeelsLike = weatherData.FeelsLike;
                cityWheater.Humidity = weatherData.Humidity;
                cityWheater.Pressure = weatherData.Pressure;
                cityWheater.WindSpeed = weatherData.WindSpeed;
                cityWheater.Description = weatherData.Description;
                cityWheater.Icon = weatherData.Icon;

                listCityWheater.Add(cityWheater);
            }

            return listCityWheater;
        }

        // NOWA METODA: Pobieranie danych pogodowych dla miasta
        private async Task<WeatherData> GetWeatherForCityAsync(City city)
        {
            WeatherData? weather = await dataBaseRepository.RetriveWheaterDataAsync(city.Id, LastUpdate());
            WeatherData weatherData;
            if (weather == null)
            {
                weatherData = await weatherRepository.GetWeatherForCityAsync(city);
                await dataBaseRepository.AddWheaterAsync(weatherData);
                
            }
            else if(weather.LastUpdated< LastUpdate())
            {
                weatherData = await weatherRepository.GetWeatherForCityAsync(city);
                weatherData.Id = weather.Id;
                await dataBaseRepository.UpdateWeatherAsync(weatherData);
            }
            else
            {
                weatherData = weather;
            }

            return weatherData;
        }

        public async Task<WheaterForecastVM> GetWeatherForWeekAsync(int cityId)
        {
            WheaterForecastVM wheaterForecastVM = new WheaterForecastVM();

            City? city = await dataBaseRepository.ReadCityAsync(cityId);
            if (city == null)
            {
                city = new City();
                city.Name = "MIEJSCOWOŚĆ NIE ODNALEZIONA";
                city.CantryCode = "";
                wheaterForecastVM.City = city;
                return wheaterForecastVM;
            }
            wheaterForecastVM.City = city;

            bool dataIsUpdated = true;

            List<WeatherData> weatherListFromaBase = new List<WeatherData>();

            DateTime begin = LastUpdate();
            DateTime end = begin.AddDays(7);//bez tej daty

            List<WeatherData> weatherList = new List<WeatherData>();

            List<WeatherData> weatherListFromBase = await dataBaseRepository.RetriveWheaterDataAsync(cityId, begin, end);
            if (weatherListFromBase.Count < 40 || weatherListFromBase.Any(w => w.LastUpdated < LastUpdate()))
            {
                weatherList = await weatherRepository.GetWeatherForWeekAsync(city);
                foreach (WeatherData weather in weatherList)
                {
                    WeatherData? weatherBase = weatherListFromBase.Find(w => w.Date == weather.Date);
                    if (weatherBase == null)
                    {
                        await dataBaseRepository.AddWheaterAsync(weather);
                    }
                    else if (weatherBase.LastUpdated < LastUpdate())
                    {
                        weather.Id = weatherBase.Id;
                        await dataBaseRepository.UpdateWeatherAsync(weather);
                    }
                }

            }
            else
            {
                weatherList = weatherListFromBase;
            }


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


            return wheaterForecastVM;
        }


        // NOWA METODA: Usunięcie miasta i jego danych pogodowych
        public async Task DeleteCityAsync(int cityId)
        {
            await dataBaseRepository.DeleteCityAsync(cityId);
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