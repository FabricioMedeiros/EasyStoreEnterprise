
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ESE.Core.Messages.Integration;
using ESE.Authentication.API.Models;
using ESE.Authentication.API.Services;
using ESE.MessageBus;
using ESE.WebAPI.Core.Controllers;

namespace ESE.Authentication.API.Controllers
{
    [Route("api/authentication")]
    public class AuthController : MainController
    {
        private readonly AuthenticationService _authenticationService;
        private readonly IMessageBus _bus;

        public AuthController(AuthenticationService authenticationService, IMessageBus bus)
        {
            _authenticationService = authenticationService;
            _bus = bus;
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

            var result = await _authenticationService.UserManager.CreateAsync(user, userRegister.Password);

            if (result.Succeeded)
            {
                var clientResult = await RegisterClient(userRegister); 

                if (!clientResult.ValidationResult.IsValid)
                {
                    await _authenticationService.UserManager.DeleteAsync(user);
                    return CustomResponse(clientResult.ValidationResult);
                }

                return CustomResponse(await _authenticationService.CreateJwt(userRegister.Email));
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

            var result = await _authenticationService.SignInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);

            if (result.Succeeded)
            {
                return CustomResponse(await _authenticationService.CreateJwt(userLogin.Email));
            }

            if (result.IsLockedOut)
            {
                AddProcessingError("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse();
            }

            AddProcessingError("Usuário ou Senha incorretos");
            return CustomResponse();
        }

        private async Task<ResponseMessage> RegisterClient(UserRegister userRegister)
        {
            var user = await _authenticationService.UserManager.FindByEmailAsync(userRegister.Email);

            var userRegistered = new UserRegisteredIntegrationEvent(Guid.Parse(user.Id), userRegister.Name, userRegister.Email, userRegister.Cpf);

            try
            {
                return await _bus.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(userRegistered);
            }
            catch 
            {
               await _authenticationService.UserManager.DeleteAsync(user);
               throw;
            }          
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                AddProcessingError("Refresh Token inválido");
                return CustomResponse();
            }

            var token = await _authenticationService.GetRefreshToken(Guid.Parse(refreshToken));

            if (token is null)
            {
                AddProcessingError("Refresh Token expirado");
                return CustomResponse();
            }

            return CustomResponse(await _authenticationService.CreateJwt(token.Username));
        }
    }
}
