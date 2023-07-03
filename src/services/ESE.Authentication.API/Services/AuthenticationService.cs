using ESE.Authentication.API.Data;
using ESE.Authentication.API.Extensions;
using ESE.Authentication.API.Models;
using ESE.WebAPI.Core.Authentication;
using ESE.WebAPI.Core.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.JwtSigningCredentials.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESE.Authentication.API.Services
{
    public class AuthenticationService
    {
        public readonly SignInManager<IdentityUser> SignInManager;
        public readonly UserManager<IdentityUser> UserManager;
        private readonly AppSettings _appSettings;
        private readonly AppTokenSettings _appTokenSettings;
        private readonly ApplicationDbContext _dbContext;

        private readonly IJsonWebKeySetService _jwksService;
        private readonly IAspNetUser _aspNetUser;

        public AuthenticationService(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings,
            IOptions<AppTokenSettings> appTokenSettingsSettings,
            ApplicationDbContext context,
            IJsonWebKeySetService jwksService,
            IAspNetUser aspNetUser)
        {
            SignInManager = signInManager;
            UserManager = userManager;
            _appSettings = appSettings.Value;
            _appTokenSettings = appTokenSettingsSettings.Value;
            _jwksService = jwksService;
            _aspNetUser = aspNetUser;
            _dbContext = context;
        }

        public async Task<UserResponseLogin> CreateJwt(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            var claims = await UserManager.GetClaimsAsync(user);

            ClaimsIdentity identityClaims = await GetUserClaims(user, claims);                

            string encodedToken = EncondeToken(identityClaims);
            var refreshToken = await CreateRefreshToken(email);

            return UserResponseLogin(user, claims, encodedToken, refreshToken);
        }

        public async Task<ClaimsIdentity> GetUserClaims(IdentityUser user, IList<Claim> claims)
        {
            var userRoles = await UserManager.GetRolesAsync(user);

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
            var currentIssuer = $"{_aspNetUser.GetHttpContext().Request.Scheme}://{_aspNetUser.GetHttpContext().Request.Host}";
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

        public UserResponseLogin UserResponseLogin(IdentityUser user, ICollection<Claim> claims, string encodedToken, RefreshToken refreshToken)
        {
            return new UserResponseLogin
            {
                AccessToken = encodedToken,
                RefreshToken = refreshToken.Token,
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

        private async Task<RefreshToken> CreateRefreshToken(string email)
        {
            var refreshToken = new RefreshToken
            {
                Username = email,
                ExpirationDate = DateTime.UtcNow.AddHours(_appTokenSettings.RefreshTokenExpiration)
            };

            _dbContext.RefreshTokens.RemoveRange(_dbContext.RefreshTokens.Where(u => u.Username == email));
            await _dbContext.RefreshTokens.AddAsync(refreshToken);

            await _dbContext.SaveChangesAsync();

            return refreshToken;
        }
        public async Task<RefreshToken> GetRefreshToken(Guid refreshToken)
        {
            var token = await _dbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(u => u.Token == refreshToken);

            return token != null && token.ExpirationDate.ToLocalTime() > DateTime.Now ? token : null;
        }
    }
}
