using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Service;

namespace SerwisPogodowy.Controllers
{
    public class CityController : Controller
    {
        private ICityService cityService;
        
        public CityController(ICityService cityService)
        {
            this.cityService = cityService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            return View(await cityService.ReadAllLocalizationsAsync());
        }

        public async Task<IActionResult> AddAsync(CitySearchVM? citySearch = null)
        {
            if(citySearch == null)
            {
                citySearch = new CitySearchVM();
            }
            else if(!string.IsNullOrEmpty(citySearch.CityName))
            {
                citySearch.Cities = await cityService.SelectCity(citySearch.CityName);
            }
            return View(citySearch);
        }
        [HttpPost]
        public async Task<IActionResult> SelectCityAsync(string cityName)
        {
            List<City> cities = new List<City>();
            CitySearchVM citySearch = new CitySearchVM();
            citySearch.CityName = cityName;
            return RedirectToAction("Add", "City", citySearch);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(City city)
        {
            
            await cityService.AddCityAsync(city);
            return RedirectToAction("Index", "City");
        }

    }
}
