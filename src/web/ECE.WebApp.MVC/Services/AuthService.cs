using ECE.WebApp.MVC.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace ECE.WebApp.MVC.Services
{
    public class AuthService : Service, IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserResponseLogin> Login(UserLogin userLogin)
        {
            var loginContent = JsonSerialize(userLogin);          

            var response = await _httpClient.PostAsync("https://localhost:44334/api/authentication/login", loginContent);
            
            if (!CheckErrorsResponse(response))  {
                return new UserResponseLogin
                {
                    ResponseResult = await DeserializeObjectResponse<ResponseResult>(response)
                };
            }

            return await DeserializeObjectResponse<UserResponseLogin>(response);
        }

        public async Task<UserResponseLogin> Register(UserRegister userRegister)
        {
            var registerContent = JsonSerialize(userRegister);

            var response = await _httpClient.PostAsync("https://localhost:44334/api/authentication/register", registerContent);
                        
            if (!CheckErrorsResponse(response))
            {
                return new UserResponseLogin
                {
                    ResponseResult = await DeserializeObjectResponse<ResponseResult>(response)
                };
            }

            return await DeserializeObjectResponse<UserResponseLogin>(response);
        }
    }
}
