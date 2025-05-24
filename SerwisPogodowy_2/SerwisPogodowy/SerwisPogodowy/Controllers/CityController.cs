using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Service;
using System.ComponentModel.DataAnnotations;

namespace SerwisPogodowy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CityController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        /// <summary>
        /// Pobiera wszystkie miasta
        /// </summary>
        /// <returns>Lista wszystkich miast</returns>
        /// <response code="200">Zwraca listę miast</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<City>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<City>>> GetAllCities()
        {
            var cities = await _cityService.ReadAllLocalizationsAsync();
            return Ok(cities);
        }

        /// <summary>
        /// Wyszukuje miasta po nazwie
        /// </summary>
        /// <param name="cityName">Nazwa miasta do wyszukania</param>
        /// <returns>Lista znalezionych miast</returns>
        /// <response code="200">Zwraca listę znalezionych miast</response>
        /// <response code="400">Jeśli nazwa miasta jest pusta</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<City>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<City>>> SearchCities(
            [FromQuery, Required] string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                return BadRequest("Nazwa miasta nie może być pusta");
            }

            var cities = await _cityService.SelectCity(cityName);
            return Ok(cities);
        }

        /// <summary>
        /// Dodaje nowe miasto
        /// </summary>
        /// <param name="city">Dane miasta do dodania</param>
        /// <returns>Potwierdz dodania miasta</returns>
        /// <response code="201">Miasto zostało dodane</response>
        /// <response code="400">Jeśli dane miasta są nieprawidłowe</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCity([FromBody, Required] City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _cityService.AddCityAsync(city);
            return CreatedAtAction(nameof(GetAllCities), new { }, city);
        }

        /// <summary>
        /// Usuwa miasto po ID
        /// </summary>
        /// <param name="id">ID miasta do usunięcia</param>
        /// <returns>Potwierdzenie usunięcia</returns>
        /// <response code="204">Miasto zostało usunięte</response>
        /// <response code="404">Miasto nie zostało znalezione</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCity([FromRoute, Required] int id)
        {
            await _cityService.DeleteCityAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Pobiera prognozę pogody dla miasta
        /// </summary>
        /// <param name="id">ID miasta</param>
        /// <returns>Prognoza pogody na tydzień</returns>
        /// <response code="200">Zwraca prognozę pogody</response>
        /// <response code="404">Miasto nie zostało znalezione</response>
        [HttpGet("{id}/weather")]
        [ProducesResponseType(typeof(WheaterForecastVM), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WheaterForecastVM>> GetWeatherForecast(
            [FromRoute, Required] int id)
        {
            var forecast = await _cityService.GetWeatherForWeekAsync(id);

            if (forecast == null)
            {
                return NotFound($"Nie znaleziono miasta o ID: {id}");
            }

            return Ok(forecast);
        }
    }
}