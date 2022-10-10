using ECE.WebApp.MVC.Models;
using System.Threading.Tasks;

namespace ECE.WebApp.MVC.Services
{
    public interface IAuthService 
    {
        Task<UserResponseLogin> Login(UserLogin userLogin);

        Task<UserResponseLogin> Register(UserRegister userRegister);
    }
}
