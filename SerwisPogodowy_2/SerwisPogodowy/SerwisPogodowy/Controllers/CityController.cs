using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
           
            return View();
        }

        public IActionResult Add(CitySearchVM? citySearch = null)
        {
            if(citySearch == null)
            {
                citySearch = new CitySearchVM();
            }
       
            return View(citySearch);
        }
        [HttpPost]
        public async Task<IActionResult> SelectCityAsync(string cityName)
        {
            List<CityLocalication> cities = new List<CityLocalication>();
            CitySearchVM citySearch = new CitySearchVM();
            citySearch.Cities = await cityService.SelectCity(cityName);
            citySearch.CityName = cityName;
            return RedirectToAction("Add", "City", citySearch);// nie działa przekazanie listy miast
        }
            
        
    }
}
