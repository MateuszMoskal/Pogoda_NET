using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Service;

namespace SerwisPogodowy.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ISessionService sessionService;

        public UserController(IUserService userService, ISessionService sessionService)
        {
            this.userService = userService;
            this.sessionService = sessionService;
        }

        public IActionResult LogIn()
        {
            // Jeśli już zalogowany, przekieruj
            if (sessionService.IsLogged)
                return RedirectToAction("Index", "City");

            return View(new UserLoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn(UserLoginVM model)
        {
            // Sprawdź czy już zalogowany
            if (sessionService.IsLogged)
                return RedirectToAction("Index", "City");

            // Walidacja modelu
            if (!ModelState.IsValid)
                return View(model);

            // Podstawowa sanityzacja
            model.Email = model.Email?.Trim().ToLowerInvariant() ?? "";

            if (userService.LogIn(model))
            {
                return RedirectToAction("Index", "City");
            }
            else
            {
                model.ErrorMessage = "Błędne dane logowania";
                return View(model);
            }
        }

        public IActionResult LogOff()
        {
            sessionService.User = null;
            return RedirectToAction("LogIn");
        }

        public IActionResult Register()
        {
            // Jeśli już zalogowany, przekieruj
            if (sessionService.IsLogged)
                return RedirectToAction("Index", "City");

            return View(new UserRegisterVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserRegisterVM model)
        {
            // Sprawdź czy już zalogowany
            if (sessionService.IsLogged)
                return RedirectToAction("Index", "City");

            // Walidacja modelu
            if (!ModelState.IsValid)
                return View(model);

            // Podstawowa sanityzacja
            model.Email = model.Email?.Trim().ToLowerInvariant() ?? "";

            if (userService.Register(model))
            {
                return RedirectToAction("Index", "City");
            }
            else
            {
                // ErrorMessage będzie ustawiony w serwisie
                return View(model);
            }
        }
    }
}
