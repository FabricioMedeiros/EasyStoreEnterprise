using ECE.WebApp.MVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECE.WebApp.MVC.Services
{
    public interface IAuthenticationService 
    {
        Task<string> Login(UserLogin userLogin);

        Task<string> Register(UserRegister userRegister);
    }
}
