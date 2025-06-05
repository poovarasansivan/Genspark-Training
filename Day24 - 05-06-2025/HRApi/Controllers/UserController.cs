using System.Threading.Tasks;
using HRApi.Interfaces;
using HRApi.Models;
using HRApi.Services;
using HRApi.Models.DTOs.FileHandlingDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HRApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userServices;

        public UserController(IUserService userService)
        {
            _userServices = userService;
        }

        [HttpGet("{name}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(string name)
        {
            var result = await _userServices.GetUserByName(name);
            if (result != null)
                return Ok(result);
            return NotFound("User not found");
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] UserAddRequestDto user)
        {
            try
            {
                var addUser = await _userServices.AddUser(user);
                if (addUser != null)
                    return Created("", addUser);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
