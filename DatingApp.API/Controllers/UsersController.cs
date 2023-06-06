using System.Net.Sockets;
using System.Security.Claims;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Extensions;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseApiController
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(ILogger<UsersController> logger, 
                    IUserRepository userRepository, IMapper mapper, IPhotoService photoService ) 
                    : base(logger)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsers();
            var usersResponse = _mapper.Map<IEnumerable<MemberDto>>(users);
            return Ok(usersResponse);
        }

        // [HttpGet("{id}")]
        // public async Task<ActionResult<AppUser>> GetById(int id)
        // {
        //     var user = await _userRepository.GetUserByIdAsync(id);
        //     var userResponse = _mapper.Map<MemberDto>(user);
        //     return Ok(userResponse);    
        // }

        [Authorize(Roles = "Manager")]
        [HttpGet("{username}")]
        public async Task<ActionResult<AppUser>> GetByName(string username)
        {
            var user = await _userRepository.GetUserByNameAsync(username);
            var userResponse = _mapper.Map<MemberDto>(user);
            return Ok(userResponse);    
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdate){
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());
            if(user == null) return NotFound();

            // user.City = memberUpdate.City;
            // user.Country = memberUpdate.Country;
            // user.Introduction = memberUpdate.Introduction;
            // user.LookingFor = memberUpdate.LookingFor;
            _mapper.Map(memberUpdate, user);

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Faild to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file){
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());
            if(user is null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null)
            return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(!user.Photos.Any()) 
                photo.IsMain = true;

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
                return CreatedAtAction(nameof(GetByName), 
                                        new {username = user.UserName}, 
                                        _mapper.Map<PhotoDto>(photo));

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());
            if(user is null)
                return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo is null)
                return NotFound();

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null)
                currentMain.IsMain = false;

            photo.IsMain = true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if(photo is null) return NotFound();

            if(photo.IsMain) return BadRequest("You cannot delete your main photo");
            if(photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Poblem deleting photo");
        }
    }
}