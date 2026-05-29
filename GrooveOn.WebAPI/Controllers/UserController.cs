using System.Security.Claims;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Exceptions;
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

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("")]
        public override Task<PagedResult<UserResponse>> Get([FromQuery] UserSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
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

            return Ok(new { message = "If the email exists, a reset code has been sent." });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _userService.ResetPasswordWithTokenAsync(request.Token, request.NewPassword);

            return Ok(new { message = "Password has been reset successfully." });
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("current-user/has-premium")]
        public async Task<bool> CurrentUserHasPremium()
        {
            return await _userService.CurrentUserHasPremiumAsync();
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
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

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpPut("{id}")]
        public override async Task<UserResponse?> Update(int id, [FromBody] UserUpdateRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var loggedInUserId))
                throw new UnauthorizedAccessException("Invalid token.");

            if (!User.IsInRole(Roles.Admin) && id != loggedInUserId)
                throw new ForbiddenException("You can only update your own profile.");

            return await base.Update(id, request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}/roles")]
        public async Task<IActionResult> AssignRoles(int id, [FromBody] List<int> roleIds)
        {
            await _userService.AssignRolesAsync(id, roleIds);
            return Ok(new { message = "Roles updated successfully." });
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }
    }
}
