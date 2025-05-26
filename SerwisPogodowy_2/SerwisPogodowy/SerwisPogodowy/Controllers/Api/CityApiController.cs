using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Service;

namespace SerwisPogodowy.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CityApiController : ControllerBase
    {
        private readonly ICityService cityService;

        public CityApiController(ICityService cityService)
        {
            this.cityService = cityService;
        }

        /// <summary>
        /// Pobiera wszystkie miasta
        /// </summary>
        /// <returns>Lista miast</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<City>>> GetAllCities()
        {
            var cities = await cityService.ReadAllLocalizationsAsync();
            return Ok(cities);
        }

        /// <summary>
        /// Wyszukuje miasta po nazwie
        /// </summary>
        /// <param name="cityName">Nazwa miasta</param>
        /// <returns>Lista znalezionych miast</returns>
        [HttpGet("search/{cityName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<City>>> SearchCities(string cityName)
        {
            var cities = await cityService.SelectCity(cityName);
            if (cities == null || !cities.Any())
            {
                return NotFound($"Nie znaleziono miast o nazwie: {cityName}");
            }
            return Ok(cities);
        }

        /// <summary>
        /// Dodaje nowe miasto
        /// </summary>
        /// <param name="city">Dane miasta</param>
        /// <returns>Dodane miasto</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<City>> AddCity([FromBody] City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await cityService.AddCityAsync(city);
            return CreatedAtAction(nameof(GetWeatherForecast), new { id = city.Id }, city);
        }

        /// <summary>
        /// Usuwa miasto
        /// </summary>
        /// <param name="id">ID miasta</param>
        /// <returns>Status operacji</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCity(int id)
        {
            await cityService.DeleteCityAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Pobiera prognozê pogody dla miasta
        /// </summary>
        /// <param name="id">ID miasta</param>
        /// <returns>Prognoza pogody</returns>
        [HttpGet("{id}/weather")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WheaterForecastVM>> GetWeatherForecast(int id)
        {
            var forecast = await cityService.GetWeatherForWeekAsync(id);
            if (forecast == null)
            {
                return NotFound($"Nie znaleziono prognozy dla miasta o ID: {id}");
            }
            return Ok(forecast);
        }
    }
}