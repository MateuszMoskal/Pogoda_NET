using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models;

namespace SerwisPogodowy.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HomeApiController : ControllerBase
    {
        private readonly ILogger<HomeApiController> _logger;

        public HomeApiController(ILogger<HomeApiController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sprawdza status aplikacji
        /// </summary>
        /// <returns>Status aplikacji</returns>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> GetStatus()
        {
            return Ok(new
            {
                Status = "OK",
                Timestamp = DateTime.Now,
                Application = "Serwis Pogodowy"
            });
        }

        /// <summary>
        /// Pobiera informacje o aplikacji
        /// </summary>
        /// <returns>Informacje o aplikacji</returns>
        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> GetInfo()
        {
            return Ok(new
            {
                Name = "Serwis Pogodowy",
                Version = "1.0.0",
                Description = "Aplikacja do zarz¹dzania prognoz¹ pogody"
            });
        }
    }
}