using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatingApp.API.Controllers
{
    
    public class AdminController : BaseApiController
    {
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(ILogger<AdminController> logger, UserManager<AppUser> userManager) : base(logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {
            var users = await _userManager.Users.OrderBy(u => u.UserName)
            .Select(u => new {
                u.Id,
                Name = u.UserName,
                Role = u.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userName}")]
        public async Task<ActionResult> EditRoles(string userName, [FromQuery] string roles)
        {
            if(string.IsNullOrEmpty(roles))
                return BadRequest("Selected roles");

            var selectedRoles = roles.Split(',').ToArray();

            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if(!result.Succeeded)
                return BadRequest("Faild to add to role");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if(!result.Succeeded)
                return BadRequest("Faild to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotoForModerator()
        {
            return Ok();
        }
    }
}