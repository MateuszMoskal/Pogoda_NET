using System.Security.Cryptography;
using System.Text;
using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;
using SerwisPogodowy.Repositories;

namespace SerwisPogodowy.Service
{
    public class UserService : IUserService
    {
        private readonly ISessionService sessionService;
        private readonly IDataBaseRepository dataBaseRepository;

        public UserService(ISessionService sessionService, IDataBaseRepository dataBaseRepository)
        {
            this.sessionService = sessionService;
            this.dataBaseRepository = dataBaseRepository;
        }

        public bool LogIn(UserLoginVM user)
        {
            // Podstawowa walidacja
            if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
                return false;

            try
            {
                User? userFromBase = dataBaseRepository.ReadUser(user.Email, HashPassword(user.Password));

                if (userFromBase != null)
                {
                    sessionService.UserFromBase = userFromBase;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool Register(UserRegisterVM user)
        {
            // Podstawowa walidacja
            if (user == null || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
            {
                user.ErrorMessage = "Email i hasło są wymagane";
                return false;
            }

            if (user.Password != user.PasswordConfirm)
            {
                user.ErrorMessage = "Hasła muszą być identyczne";
                return false;
            }

            try
            {
                if (dataBaseRepository.UserExiest(user.Email))
                {
                    user.ErrorMessage = "Użytkownik o podanym emailu już istnieje";
                    return false;
                }

                User userDataBase = dataBaseRepository.CreateUser(user.Email, HashPassword(user.Password));
                sessionService.UserFromBase = userDataBase;
                return true;
            }
            catch
            {
                user.ErrorMessage = "Wystąpił błąd podczas rejestracji";
                return false;
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