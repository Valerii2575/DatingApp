using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Interfaces;

namespace DatingApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(ILogger<AccountController> logger, DataContext context, ITokenService tokenService)
         : base(logger)
        {
            _logger = logger;
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest appUser)
        {
            if(await UserExsist(appUser.UserName))
            {
                return BadRequest("User whith name already exist");
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser{
                UserName = appUser.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(appUser.UserPassword)),
                PasswordSalt = hmac.Key
            };
 
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        } 

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest userRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == userRequest.UserName);
            if(user == null){
                return Unauthorized();
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userRequest.UserPassword));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized();
                }
            }

            return new UserDto{
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExsist(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        }
    }
}