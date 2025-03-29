using Microsoft.AspNetCore.Mvc;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Service;

namespace SerwisPogodowy.Controllers
{
    public class UserController : Controller
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
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
                return RedirectToAction("CurrentActivity", "ASD");//do poprawy
            }
            else
            {
                model.ErrorMessage = "Błędne dane logowania";
                return View(model);
            }
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
