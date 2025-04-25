using SerwisPogodowy.Models;

namespace SerwisPogodowy.Repositories
{
    public interface IDataBaseRepository
    {
        User? ReadUser(string email, string hashPassword);
        bool UserExiest(string email);

        User CreateUser(string email, string hashPassword);

        Task AddCityAsync(City city, int userId);
    }
}
