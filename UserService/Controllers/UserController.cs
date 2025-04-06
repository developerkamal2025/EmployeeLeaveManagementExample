using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Authentication;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJWTToken _token;
        public UserController(IUserService userService, IJWTToken token) {
            _userService = userService;
            _token = token; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser user)
        {
            int id = await _userService.Register(new UserEntity
            {
                UserName = user.UserName,
                Password = user.Password,
                Role = user.Role,
                BalanceLeave = 12
            });

            if (id == 0)
                return BadRequest("User Already Exist");

            return Ok("User registered successfully.");
        }

        [HttpGet("Login")]
        public async Task<ActionResult> Login(string userName, string password)
        {
            try
            {
                var user = await _userService.Login(userName, password);

                if (user != null)
                {
                    var token = await _token.GenerateJWT(user);

                    return Ok(new { Status = 200, Token = token, Message = "Login Successful" });
                }
                else
                {
                    return Ok(new { Status = 409, Message = "Login Failed" });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("GetUserList")]
        public async Task<ActionResult> GetUserList()
        {
            try
            {
                var users = await _userService.GetUserList();

                return Ok(new {Status = 200, Users = users});
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize]
        [HttpGet("GetUserById")]
        public async Task<ActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);

                return Ok(new { Status = 200, User = user });
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
