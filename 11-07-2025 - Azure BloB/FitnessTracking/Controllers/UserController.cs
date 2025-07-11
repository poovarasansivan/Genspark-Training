using Microsoft.AspNetCore.Mvc;
using FitnessTracking.Models;
using FitnessTracking.Interfaces;
using FitnessTracking.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using FitnessTracking.Misc;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using FitnessTracking.Contexts;
using Microsoft.Extensions.Caching.Memory;

namespace FitnessTracking.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly FitnessContext _context;
        private readonly IMemoryCache _cache;

        public UserController(IUserService userService, ILogger<UserController> logger, FitnessContext context, IMemoryCache cache)
        {
            _userService = userService;
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin, Coach")]
        public async Task<ActionResult<UserModel?>> GetByUserId(Guid id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", id);
                return NotFound();
            }

            _logger.LogInformation("User fetched successfully for ID: {UserId}", id);
            return Ok(ResponseHandler.Success(user, "User fetched successfully"));
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Coach")]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users");

            var users = await _userService.GetAllUsersAsync();
            if (users == null)
            {
                _logger.LogWarning("No users found in the database");
                return NotFound();
            }

            _logger.LogInformation("All users fetched successfully");
            return Ok(ResponseHandler.Success(users, "All users fetched successfully"));
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddUser([FromBody] UserRegisterDto user)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user model received");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new user with email: {Email}", user.Email);

            await _userService.AddUserAsync(user);

            _logger.LogInformation("User created successfully: {Email}", user.Email);
            return Ok(ResponseHandler.Success("User created successfully"));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("UpdateUser called with empty ID");
                throw new CustomeExceptionHandler("User Id cannot be empty", 404);
            }

            _logger.LogInformation("Updating user with ID: {UserId}", id);
            await _userService.UpdateUserAsync(id, updateUserDto);
            _logger.LogInformation("User updated successfully for ID: {UserId}", id);

            return Ok(ResponseHandler.Success("User updated successfully"));
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] UpdatePasswordDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Token) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Token and password are required.");
            }

            if (!_cache.TryGetValue(model.Token, out Guid userId))
            {
                _logger.LogWarning("Invalid or expired reset token: {Token}", model.Token);
                return BadRequest("Invalid or expired reset token.");
            }

            await _userService.UpdatePasswordAsync(userId, model);

            _cache.Remove(model.Token);

            _logger.LogInformation("Password reset successfully for user ID: {UserId}", userId);
            return Ok(ResponseHandler.Success("Password has been reset successfully."));
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("DeleteUser called with empty ID");
                throw new CustomeExceptionHandler("User Id cannot be empty", 404);
            }

            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            await _userService.DeleteUserAsync(id);
            _logger.LogInformation("User deleted successfully for ID: {UserId}", id);

            return Ok(ResponseHandler.Success("User deleted successfully"));
        }

        [HttpPut("status/{id}")]
        [Authorize(Roles = "Admin, Coach")]
        public async Task<ActionResult> UpdateUserStatus(Guid id, [FromBody] UpdateUserStatusDto updateUserStatusDto)
        {
            if (id == Guid.Empty || updateUserStatusDto == null)
            {
                _logger.LogWarning("UpdateUserStatus called with invalid input");
                throw new CustomeExceptionHandler("User Id cannot be empty", 404);
            }

            _logger.LogInformation("Updating user status for ID: {UserId}", id);
            await _userService.UpdateUserStatusAsyn(id, updateUserStatusDto);
            _logger.LogInformation("User status updated successfully for ID: {UserId}", id);

            return Ok(ResponseHandler.Success("User status updated successfully"));
        }

        [HttpGet("paginated")]
        [Authorize(Roles = "Admin, User, Coach")]
        public async Task<ActionResult<PaginatedResult<UserModel>>> GetPaginatedAllUsersAsync([FromQuery] PaginationParameterDto paginationParameterDto)
        {
            if (paginationParameterDto == null)
            {
                _logger.LogWarning("Pagination parameters cannot be null");
                throw new CustomeExceptionHandler("Pagination parameters cannot be null", 400);
            }

            _logger.LogInformation("Fetching paginated users - Page: {Page}, PageSize: {PageSize}",
                paginationParameterDto.PageNumber, paginationParameterDto.PageSize);

            var paginatedUsers = await _userService.GetPaginatedAllUsersAsync(paginationParameterDto);

            _logger.LogInformation("Paginated users fetched successfully");
            return Ok(ResponseHandler.Success(paginatedUsers, "Paginated users fetched successfully"));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotpasswordModel forgotPasswordModel)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == forgotPasswordModel.Email);
            if (user == null)
            {
                _logger.LogWarning("Forgot password request failed: User not found for email {Email}.", forgotPasswordModel.Email);
                return NotFound("User not found");
            }
            var token = Guid.NewGuid().ToString("N");
            _cache.Set(token, user.Id, TimeSpan.FromMinutes(15));

            var resetUrl = $"http://localhost:4200/reset-password?token={token}";
            SendResetEmail(forgotPasswordModel.Email, resetUrl);
            return Ok(new { message = "Password reset link sent to your email." });
        }

        private void SendResetEmail(string email, string resetLink)
        {
            var fromAddress = new MailAddress("poovarasansivan3@gmail.com", "Fitness Tracking App");
            var toAddress = new MailAddress(email);
            const string fromPassword = "zqjn tlmu myzg bchr";
            const string subject = "Reset your password";
            string body = $"Click the link to reset your password: {resetLink}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            smtp.Send(message);
        }
    }
}
