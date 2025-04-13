using SerwisPogodowy.Models;
using System.Text;
using System.Text.Json;
namespace SerwisPogodowy.Service
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;


        public SessionUser User
        {
            get
            {
                SessionUser user = null;
                byte[] bytes = null;
                if (Session.TryGetValue("user", out bytes))
                {
                    string jsonString = Encoding.UTF8.GetString(bytes);
                    user = JsonSerializer.Deserialize<SessionUser>(jsonString);
                }

                return user;
            }
            set
            {
                string jsonString = JsonSerializer.Serialize(value);
                byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                Session.Set("user", bytes);
            }
        }


        public User UserFromBase
        {
            set
            {
                string jsonString = JsonSerializer.Serialize(Convert(value));
                byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
                Session.Set("user", bytes);
            }
        }

        public bool IsLogged
        {
            get
            {
                return User != null;
            }
        }

        private SessionUser Convert(User user)
        {
            SessionUser sessionUser = new SessionUser();
            sessionUser.Id = user.Id;
            sessionUser.Email = user.Email;
            return sessionUser;
        }

    }
}
