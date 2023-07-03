using ESE.Core.Communication;
using ESE.WebAPI.Core.User;
using ESE.WebApp.MVC.Extensions;
using ESE.WebApp.MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESE.WebApp.MVC.Services
{
    public class AuthService : Service, IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IAspNetUser _aspNetUser;
        private readonly IAuthenticationService _authenticationService;

        public AuthService(HttpClient httpClient, IOptions<AppSettings> settings, IAspNetUser aspNetUser, IAuthenticationService authenticationService)
        {
            httpClient.BaseAddress = new Uri(settings.Value.AuthenticationUrl);
            _httpClient = httpClient;
            _aspNetUser = aspNetUser;
            _authenticationService = authenticationService;
        }

        public async Task<UserResponseLogin> Login(UserLogin userLogin)
        {
            var loginContent = JsonSerialize(userLogin);          

            var response = await _httpClient.PostAsync("/api/authentication/login", loginContent);
            
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

            var response = await _httpClient.PostAsync("/api/authentication/register", registerContent);
                        
            if (!CheckErrorsResponse(response))
            {
                return new UserResponseLogin
                {
                    ResponseResult = await DeserializeObjectResponse<ResponseResult>(response)
                };
            }

            return await DeserializeObjectResponse<UserResponseLogin>(response);
        }

        public async Task<UserResponseLogin>UseRefreshToken(string refreshToken)
        {
            var refreshTokenContent = JsonSerialize(refreshToken);

            var response = await _httpClient.PostAsync("/api/authentication/refresh-token", refreshTokenContent);

            if (!CheckErrorsResponse(response))
            {
                return new UserResponseLogin
                {
                    ResponseResult = await DeserializeObjectResponse<ResponseResult>(response)
                };
            }

            return await DeserializeObjectResponse<UserResponseLogin>(response);
        }

        public async Task SignIn(UserResponseLogin response)
        {
            var token = GetTokenFormatted(response.AccessToken);

            var claims = new List<Claim>();
            claims.Add(new Claim("JWT", response.AccessToken));
            claims.Add(new Claim("RefreshToken", response.RefreshToken));
            claims.AddRange(token.Claims);

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                IsPersistent = true
            };

            await _authenticationService.SignInAsync(
                _aspNetUser.GetHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task Logout()
        {
            await _authenticationService.SignOutAsync(
                _aspNetUser.GetHttpContext(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                null);
        }

        public static JwtSecurityToken GetTokenFormatted(string jwtToken)
        {
            return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
        }

        public bool TokenExpired()
        {
            var jwt = _aspNetUser.GetUserToken();
            if (jwt is null) return false;

            var token = GetTokenFormatted(jwt);
            return token.ValidTo.ToLocalTime() < DateTime.Now;
        }

        public async Task<bool> RefreshTokenIsValid()
        {
            var response = await UseRefreshToken(_aspNetUser.GetUserRefreshToken());

            if (response.AccessToken != null && response.ResponseResult == null)
            {
                await SignIn(response);
                return true;
            }

            return false;
        }
    }
}
