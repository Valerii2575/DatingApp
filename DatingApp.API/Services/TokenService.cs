using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public UserManager<AppUser> _userManager { get; }

        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            var tokenKey = config["TokenKey"];
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        }
        public async Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}