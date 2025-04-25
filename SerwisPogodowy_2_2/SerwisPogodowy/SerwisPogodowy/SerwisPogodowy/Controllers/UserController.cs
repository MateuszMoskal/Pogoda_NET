using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Service;

namespace SerwisPogodowy.Controllers
{
    public class UserController : Controller
    {
        private IUserService userService;
        private ISessionService sessionService;


        public UserController(IUserService userService, ISessionService sessionService)
        {
            this.userService = userService;
            this.sessionService = sessionService;
        }


        public IActionResult LogIn()
        {
            return View(new UserLoginVM());
        }
        [HttpPost]
        public IActionResult LogIn(UserLoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (userService.LogIn(model))
            {
                ModelState.Remove("Login");
                ModelState.Remove("Password");
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
            return View(new UserRegisterVM());
        }


        [HttpPost]
        public IActionResult Register(UserRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.PasswordConfirm)
            {
                model.ErrorMessage = "hasło jest różne od powtorzonego hasła";
                return View(model);
            }

            if (userService.Register(model))
            {
                ModelState.Remove("Login");
                ModelState.Remove("Password");
                return RedirectToAction("CurrentActivity", "ASD");//do poprawy
            }
            else
            {
                return View(model);
            }
        }

    }
}
