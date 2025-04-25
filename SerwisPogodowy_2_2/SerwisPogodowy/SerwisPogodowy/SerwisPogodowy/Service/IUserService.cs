using SerwisPogodowy.Models;
using SerwisPogodowy.Models.ViewModels;

namespace SerwisPogodowy.Service
{
    public interface IUserService
    {
        bool LogIn(UserLoginVM user);

        bool Register(UserRegisterVM user);
    }
}
