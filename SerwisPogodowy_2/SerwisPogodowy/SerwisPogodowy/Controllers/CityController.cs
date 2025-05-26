// Controllers/CityController.cs - POPRAWIONY
using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Service;

namespace SerwisPogodowy.Controllers
{
    public class CityController : Controller
    {
        private readonly ICityService cityService;
        private readonly ISessionService sessionService;

        public CityController(ICityService cityService, ISessionService sessionService)
        {
            this.cityService = cityService;
            this.sessionService = sessionService;
        }

        // Sprawdź logowanie
        private bool CheckLogin()
        {
            if (!sessionService.IsLogged)
            {
                TempData["ErrorMessage"] = "Musisz być zalogowany";
                return false;
            }
            return true;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (!CheckLogin())
                return RedirectToAction("LogIn", "User");

            try
            {
                return View(await cityService.ReadAllLocalizationsAsync());
            }
            catch
            {
                TempData["ErrorMessage"] = "Wystąpił błąd podczas pobierania danych";
                return View(new List<CityWheaterInformationVM>());
            }
        }

        // GET: Wyświetl formularz wyszukiwania
        public async Task<IActionResult> AddAsync(CitySearchVM? citySearch = null)
        {
            if (!CheckLogin())
                return RedirectToAction("LogIn", "User");

            if (citySearch == null)
            {
                citySearch = new CitySearchVM();
            }
            else if (!string.IsNullOrWhiteSpace(citySearch.CityName))
            {
                try
                {
                    citySearch.Cities = await cityService.SelectCity(citySearch.CityName.Trim());
                }
                catch
                {
                    TempData["ErrorMessage"] = "Błąd podczas wyszukiwania miasta";
                }
            }
            return View(citySearch);
        }

        // POST: Wyszukaj miasta - POPRAWIONA NAZWA METODY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectCity(string cityName)
        {
            if (!CheckLogin())
                return RedirectToAction("LogIn", "User");

            // Podstawowa walidacja
            if (string.IsNullOrWhiteSpace(cityName))
            {
                TempData["ErrorMessage"] = "Nazwa miasta jest wymagana";
                return RedirectToAction("Add");
            }

            if (cityName.Trim().Length < 2)
            {
                TempData["ErrorMessage"] = "Nazwa miasta musi mieć co najmniej 2 znaki";
                return RedirectToAction("Add");
            }

            // Przekieruj do Add z parametrem wyszukiwania
            var citySearch = new CitySearchVM { CityName = cityName.Trim() };
            return RedirectToAction("Add", citySearch);
        }

        // POST: Dodaj miasto do bazy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(City city)
        {
            if (!CheckLogin())
                return RedirectToAction("LogIn", "User");

            // Podstawowa walidacja
            if (string.IsNullOrWhiteSpace(city.Name))
            {
                TempData["ErrorMessage"] = "Nazwa miasta jest wymagana";
                return RedirectToAction("Add");
            }

            try
            {
                await cityService.AddCityAsync(city);
                TempData["SuccessMessage"] = "Miasto zostało dodane";
            }
            catch
            {
                TempData["ErrorMessage"] = "Błąd podczas dodawania miasta";
            }

            return RedirectToAction("Index");
        }

        // POST: Wyświetl prognozę pogody
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WeatherForecast(int id)
        {
            if (!CheckLogin())
                return RedirectToAction("LogIn", "User");

            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Nieprawidłowe ID miasta";
                return RedirectToAction("Index");
            }

            try
            {
                var forecast = await cityService.GetWeatherForWeekAsync(id);
                return View(forecast);
            }
            catch
            {
                TempData["ErrorMessage"] = "Błąd podczas pobierania prognozy";
                return RedirectToAction("Index");
            }
        }

        // POST: Usuń miasto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCity(int id)
        {
            if (!CheckLogin())
                return RedirectToAction("LogIn", "User");

            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Nieprawidłowe ID miasta";
                return RedirectToAction("Index");
            }

            try
            {
                await cityService.DeleteCityAsync(id);
                TempData["SuccessMessage"] = "Miasto zostało usunięte";
            }
            catch
            {
                TempData["ErrorMessage"] = "Błąd podczas usuwania miasta";
            }

            return RedirectToAction("Index");
        }
    }
}