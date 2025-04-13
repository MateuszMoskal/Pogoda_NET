using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using SerwisPogodowy.DataBase;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using static System.Net.WebRequestMethods;

namespace SerwisPogodowy.Service
{
    public class CityService : ICityService
    {
        private readonly DataBaseContext context;
        private static readonly HttpClient client = new HttpClient();
        private const string API_CODE = "1f38869134c58999034783193cd89a1d";
        private ISessionService sessionService;

        public CityService(DataBaseContext context, ISessionService sessionService)
        {
            this.context = context;
            this.sessionService = sessionService;
        }

        public async Task<List<City>> SelectCity(string cityName)
        {

            string url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=5&appid={API_CODE}";


            var response = await client.GetStringAsync(url);

            JArray jsonArray = JArray.Parse(response);
            List<City> cities = new List<City>();
            if (jsonArray == null)
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


            //http://api.openweathermap.org/geo/1.0/direct?q={city name},{state code},{country code}&limit={limit}&appid={API key}
            //http://api.openweathermap.org/geo/1.0/direct?q=Opole&limit=5&appid=1f38869134c58999034783193cd89a1d
           // string api = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}";
            //https://api.openweathermap.org/data/2.5/weather?q=Opole&appid=1f38869134c58999034783193cd89a1d

        }

        public async Task AddCityAsync(City city)
        {
            User user = context.Users.Find(sessionService.User.Id);
            city.User = user;
            city.Id = user.Id;
            //user.Cities.Add(city);
            context.Cities.Add(city);
            await context.SaveChangesAsync();
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
                listCityWheater.Add(cityWheater);
            }
            return listCityWheater;
        }




    }
}
