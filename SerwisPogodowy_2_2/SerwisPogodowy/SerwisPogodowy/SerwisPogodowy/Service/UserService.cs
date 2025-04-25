using Microsoft.AspNetCore.Identity.Data;
using System.Text.Json;
using SerwisPogodowy.Models;
using System.Text;
using SerwisPogodowy.DataBase;
using System.Security.Cryptography;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Repositories;

namespace SerwisPogodowy.Service
{
    public class UserService : IUserService
    {
        ISessionService userService;
        IDataBaseRepository dataBaseRepository;

        public UserService(ISessionService userService, IDataBaseRepository dataBaseRepository)
        {
            this.userService = userService;
            this.dataBaseRepository = dataBaseRepository;
        }

        public bool LogIn(UserLoginVM user)
        {
            User? userFromBase = dataBaseRepository.ReadUser(user.Email, HashPassword(user.Password));

            if (userFromBase != null)
            {
                userService.UserFromBase = userFromBase;
                return true;
            }
            return false;
        }

        public bool Register(UserRegisterVM user)
        {
            if (dataBaseRepository.UserExiest(user.Email))
            {
                user.ErrorMessage = "Uzytkownik o podanym loginie jest już zarejestrowany";
                return false;
            }

            User userDataBase = dataBaseRepository.CreateUser(user.Email, HashPassword(user.Password));

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
