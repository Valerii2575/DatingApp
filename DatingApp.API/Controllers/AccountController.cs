using DatingApp.API.Entities;
using DatingApp.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ITokenService _tokenService;

        public AccountController(ILogger<AccountController> logger, UserManager<AppUser> userManager, ITokenService tokenService)
         : base(logger)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest appUser)
        {
            if(await UserExsist(appUser.UserName))
            {
                return BadRequest("User whith name already exist");
            }

            var user = new AppUser{
                UserName = appUser.UserName.ToLower(),

            };
 
            var result = await _userManager.CreateAsync(user, appUser.UserPassword);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Manager");
            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserDto{
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        } 

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequest userRequest)
        {
            var user = await _userManager.Users
                                .Include(x => x.Photos)
                                .SingleOrDefaultAsync(x => x.UserName == userRequest.UserName);
            
            if(user == null){
                return Unauthorized();
            }

            var result = await _userManager.CheckPasswordAsync(user, userRequest.UserPassword);
            if(!result) return Unauthorized();
            var token = await _tokenService.CreateToken(user);

            return new UserDto{
                UserName = user.UserName,
                Token = token,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url
            };
        }

        private async Task<bool> UserExsist(string userName)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        }
    }
}