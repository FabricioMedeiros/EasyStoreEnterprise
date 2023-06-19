using ESE.Authentication.API.Models;
using ESE.Core.Messages.Integration;
using ESE.MessageBus;
using ESE.WebAPI.Core.Authentication;
using ESE.WebAPI.Core.Controllers;
using ESE.WebAPI.Core.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ESE.Authentication.API.Controllers
{
    [Route("api/authentication")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        private readonly IJsonWebKeySetService _jwksService;
        private readonly IAspNetUser _aspNetUser;

        private readonly IMessageBus _bus;

        public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IOptions<AppSettings> appSettings, IMessageBus bus, IAspNetUser aspNetUser, IJsonWebKeySetService jwksService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _bus = bus;
            _aspNetUser = aspNetUser;
            _jwksService = jwksService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegister userRegister)
        {  
            
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = userRegister.Email,
                Email = userRegister.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, userRegister.Password);

            if (result.Succeeded)
            {
                var clientResult = await RegisterClient(userRegister); 

                if (!clientResult.ValidationResult.IsValid)
                {
                    await _userManager.DeleteAsync(user);
                    return CustomResponse(clientResult.ValidationResult);
                }

                return CustomResponse(await CreateJwt(userRegister.Email));
            }

            foreach (var error in result.Errors)
            {
                AddProcessingError(error.Description);
            }

            return CustomResponse();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLogin userLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);

            if (result.Succeeded)
            {
                return CustomResponse(await CreateJwt(userLogin.Email));
            }

            if (result.IsLockedOut)
            {
                AddProcessingError("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse();
            }

            AddProcessingError("Usuário ou Senha incorretos");
            return CustomResponse();
        }

        private async Task<UserResponseLogin> CreateJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);
            ClaimsIdentity identityClaims = await GetUserClaims(user, claims);

            string encodedToken = EncondeToken(identityClaims);
            return UserResponseLogin(user, claims, encodedToken);

        }      

        private async Task<ClaimsIdentity> GetUserClaims(IdentityUser user, IList<Claim> claims)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);
            return identityClaims;
        }

        private string EncondeToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var currentIssuer =  $"{_aspNetUser.GetHttpContext().Request.Scheme}://{_aspNetUser.GetHttpContext().Request.Host}";
            var key = _jwksService.GetCurrent(); 
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = currentIssuer,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = key
            });

            var encodedToken = tokenHandler.WriteToken(token);
            return encodedToken;
        }

        private UserResponseLogin UserResponseLogin(IdentityUser user, ICollection<Claim> claims, string encodedToken)
        {
            return new UserResponseLogin
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(1).TotalSeconds,
                UserToken = new UserToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UserClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private async Task<ResponseMessage> RegisterClient(UserRegister userRegister)
        {
            var user = await _userManager.FindByEmailAsync(userRegister.Email);

            var userRegistered = new UserRegisteredIntegrationEvent(Guid.Parse(user.Id), userRegister.Name, userRegister.Email, userRegister.Cpf);

            try
            {
                return await _bus.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(userRegistered);
            }
            catch 
            {
               await _userManager.DeleteAsync(user);
               throw;
            }          
        }
    }
}
