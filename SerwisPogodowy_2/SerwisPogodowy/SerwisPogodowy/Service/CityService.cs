using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using SerwisPogodowy.Models.ViewModels;
using System.Text.Json.Nodes;
using static System.Net.WebRequestMethods;

namespace SerwisPogodowy.Service
{
    public class CityService : ICityService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string API_CODE = "a3e2d4392b5adb7a2152a674c8caf8dc";
        public async Task<List<CityLocalication>> SelectCity(string cityName)
        {

            string url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=5&appid={API_CODE}";


            var response = await client.GetStringAsync(url);

            JArray jsonArray = JArray.Parse(response);
            List<CityLocalication> cities = new List<CityLocalication>();
            if (jsonArray == null)
            {
                return cities;
            }

            foreach (var locationJson in jsonArray)
            {
                CityLocalication city = new CityLocalication();
                city.Name = locationJson["name"]?.ToString();
                city.Country = locationJson["country"]?.ToString();
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




    }
}
