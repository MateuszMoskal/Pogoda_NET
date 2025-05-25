using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Service;

namespace SerwisPogodowy.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly ISessionService sessionService;

        public UserApiController(IUserService userService, ISessionService sessionService)
        {
            this.userService = userService;
            this.sessionService = sessionService;
        }

        /// <summary>
        /// Loguje u¿ytkownika do systemu
        /// </summary>
        /// <param name="model">Dane logowania u¿ytkownika</param>
        /// <returns>Wynik logowania</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<object> LogIn([FromBody] UserLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userService.LogIn(model))
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Logowanie pomyœlne",
                    Email = model.Email,
                    RedirectUrl = "/City"
                });
            }
            else
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "B³êdne dane logowania"
                });
            }
        }

        /// <summary>
        /// Wylogowuje u¿ytkownika z systemu
        /// </summary>
        /// <returns>Potwierdzenie wylogowania</returns>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> LogOut()
        {
            sessionService.User = null;
            return Ok(new
            {
                Success = true,
                Message = "Wylogowanie pomyœlne",
                RedirectUrl = "/User/LogIn"
            });
        }

        /// <summary>
        /// Rejestruje nowego u¿ytkownika
        /// </summary>
        /// <param name="model">Dane rejestracji u¿ytkownika</param>
        /// <returns>Wynik rejestracji</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<object> Register([FromBody] UserRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Password != model.PasswordConfirm)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Has³o jest ró¿ne od powtorzonego has³a"
                });
            }

            if (userService.Register(model))
            {
                return Created("", new
                {
                    Success = true,
                    Message = "Rejestracja pomyœlna",
                    Email = model.Email
                });
            }
            else
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "B³¹d podczas rejestracji"
                });
            }
        }

        /// <summary>
        /// Pobiera informacje o aktualnie zalogowanym u¿ytkowniku
        /// </summary>
        /// <returns>Informacje o u¿ytkowniku</returns>
        [HttpGet("current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<object> GetCurrentUser()
        {
            var currentUser = sessionService.User;
            if (currentUser == null)
            {
                return Unauthorized(new
                {
                    IsLoggedIn = false,
                    Message = "U¿ytkownik nie jest zalogowany"
                });
            }

            return Ok(new
            {
                Email = currentUser.Email,
                Id = currentUser.Id,
                IsLoggedIn = true
            });
        }

        /// <summary>
        /// Sprawdza status zalogowania
        /// </summary>
        /// <returns>Status zalogowania</returns>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> GetLoginStatus()
        {
            var isLoggedIn = sessionService.User != null;
            return Ok(new
            {
                IsLoggedIn = isLoggedIn,
                Message = isLoggedIn ? "U¿ytkownik jest zalogowany" : "U¿ytkownik nie jest zalogowany"
            });
        }
    }
}