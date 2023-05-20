using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Entities;
using DatingApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public UsersController(ILogger<UsersController> logger, 
                    IUserRepository userRepository, IMapper mapper ) 
                    : base(logger)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

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

        [HttpGet("{username}")]
        public async Task<ActionResult<AppUser>> GetByName(string username)
        {
            var user = await _userRepository.GetUserByNameAsync(username);
            var userResponse = _mapper.Map<MemberDto>(user);
            return Ok(userResponse);    
        }
    }
}