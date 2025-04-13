using Microsoft.AspNetCore.Identity.Data;
using System.Text.Json;
using SerwisPogodowy.Models;
using System.Text;
using SerwisPogodowy.DataBase;
using System.Security.Cryptography;
using SerwisPogodowy.Models.ViewModels;

namespace SerwisPogodowy.Service
{
    public class UserService : IUserService
    {
        private readonly DataBaseContext context;

        private readonly IHttpContextAccessor httpContextAccessor;

        ISessionService userService;

        public UserService(DataBaseContext context, ISessionService userService)
        {
            this.userService = userService;
            this.context = context;
        }

        public bool LogIn(UserLoginVM user)
        {
            User? userFromBase = context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == HashPassword(user.Password));

            if (userFromBase != null)
            {
                userService.UserFromBase = userFromBase;
                return true;
            }
            return false;
        }

        public bool Register(UserRegisterVM user)
        {
            if (context.Users.Any(u => u.Email == user.Email))
            {
                user.ErrorMessage = "Uzytkownik o podanym loginie jest już zarejestrowany";
                return false;
            }

            User userDataBase = new User();
            userDataBase.Email = user.Email;
            userDataBase.Password = HashPassword(user.Password);
            context.Users.Add(userDataBase);
            context.SaveChanges();

            userService.UserFromBase = userDataBase;
            return true;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        
    }
}
