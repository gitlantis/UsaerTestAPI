using UserTestMonnitorAPI.DBModels;
using UserTestMonnitorAPI.Models;
using UserTestMonnitorAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserTestMonnitorAPI.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserModel model)
        {
            var result = await _userService.Authorize(model);

            if (result == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] OrgUserModel model)
        {
            var result = await _userService.AddUser(model);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsers([FromBody] List<Guid?> guids)
        {
            var result = await _userService.DeleteUsers(guids);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userService.GetUsers();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            return Ok(token);
        }

        [HttpPost]
        public async Task<IActionResult> BlockUsers([FromBody] List<Guid?> guids)
        {
            var result = await _userService.BlockUsers(guids, true);
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> UnblockUsers([FromBody] List<Guid?> guids)
        {
            var result = await _userService.BlockUsers(guids, false);
            return Ok(result);
        }
    }
}
