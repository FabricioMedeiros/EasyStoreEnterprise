using ESE.WebApp.MVC.Models;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Services
{
    public interface IAuthService 
    {
        Task<UserResponseLogin> Login(UserLogin userLogin);

        Task<UserResponseLogin> Register(UserRegister userRegister);
    }
}
