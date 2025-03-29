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

        public UserService(DataBaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
        }

        public bool LogIn(UserLoginVM user)
        {
            User? userFromBase = context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == HashPassword(user.Password));

            if (userFromBase != null)
            {
                UserFromSession = userFromBase;
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
            UserFromSession = userDataBase;

            return true;
        }

        public bool Create(UserLoginVM user)
        {
            User? userFromBase = context.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == HashPassword(user.Password));

            if (userFromBase != null)
            {
                UserFromSession = userFromBase;
                return true;
            }
            return false;
        }


        private ISession Session => httpContextAccessor.HttpContext.Session;
        public User UserFromSession 
        {
            get
            {
                User user = null;
                byte[] bytes = null;
                if (Session.TryGetValue("user", out bytes))
                {
                    string jsonString = Encoding.UTF8.GetString(bytes);
                    user = JsonSerializer.Deserialize<User>(jsonString);
                }

                return user;
            }
            private set
            {
                string jsonString = JsonSerializer.Serialize(value);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                Session.Set("user", bytes);
            }
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
