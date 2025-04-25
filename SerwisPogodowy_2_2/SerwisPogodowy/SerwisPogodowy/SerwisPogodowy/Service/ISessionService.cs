using SerwisPogodowy.Models;
using System.Text;

namespace SerwisPogodowy.Service
{
    public interface ISessionService
    {
        SessionUser User
        {
            get;
            set;
        }

        User UserFromBase
        {
            set;
        }

        bool IsLogged
        {
            get;
        }
    }
}
