using System.Security.Claims;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class UserController : BaseCRUDController<UserResponse, UserSearchObject, UserInsertRequest, UserUpdateRequest>
    {
        private readonly IUserService _userService;
        public UserController(IUserService service) : base(service)
        {
            _userService = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public override Task<PagedResult<UserResponse>> Get([FromQuery] UserSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public override Task<UserResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<LoginResponse> Login([FromBody] LoginRequest request)
        {
            return await _userService.LoginAsync(request);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request)
        {
            await _userService.ForgotPasswordAsync(request.Email);

            return Ok("If the email exists, a password reset link has been sent.");
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("current-user/has-premium")]
        public async Task<bool> CurrentUserHasPremium()
        {
            return await _userService.CurrentUserHasPremiumAsync();
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token.");

            await _userService.ChangePasswordAsync(userId, request);

            return Ok(new { message = "Password has been changed successfully." });
        }

        [AllowAnonymous]
        [HttpPost]
        public override Task<UserResponse> Create([FromBody] UserInsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut("{id}")]
        public override Task<UserResponse?> Update(int id, [FromBody] UserUpdateRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }
    }
}
