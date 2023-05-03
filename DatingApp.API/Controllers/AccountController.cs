using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using DatingApp.API.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly DataContext _context;

        public AccountController(ILogger<AccountController> logger, DataContext context)
         : base(logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register([FromBody] AppUserRequest appUser)
        {
            using var hmac = new HMACSHA512();

            var user = new AppUser{
                UserName = appUser.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(appUser.UserPassword)),
                PasswordSalt = hmac.Key
            };
 
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        } 
    }
}