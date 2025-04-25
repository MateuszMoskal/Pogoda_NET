using SerwisPogodowy.DataBase;
using SerwisPogodowy.Models;
using SerwisPogodowy.Service;

namespace SerwisPogodowy.Repositories
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private readonly DataBaseContext context;

        public DataBaseRepository(DataBaseContext context)
        {
            this.context = context;
        }

        public User? ReadUser(string email, string hashPassword)
        {
            User? userFromBase = context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashPassword);
            return userFromBase;
        }

        public bool UserExiest(string email)
        {
            return context.Users.Any(u => u.Email == email);
        }

        public User CreateUser(string email, string hashPassword)
        {
            User user = new User();
            user.Email = email;
            user.Password = hashPassword;
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public async Task AddCityAsync(City city, int userId)
        {
            User user = await context.Users.FindAsync(userId);
            city.User = user;
            city.UserId = user.Id; // POPRAWKA: city.UserId = user.Id zamiast city.Id = user.Id

            context.Cities.Add(city);
            await context.SaveChangesAsync();
        }
    }
}
