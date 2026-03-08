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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<LoginResponse> Login([FromBody] LoginRequest request)
        {
            return await _userService.LoginAsync(request);
        }

        // [AllowAnonymous]
        // [HttpPost("forgot-password")]
        // public async Task<IActionResult> ForgotPassword(
        // [FromBody] ForgotPasswordRequest request)
        // {
        //     await _userService.ForgotPasswordAsync(request.Email);

        //     return Ok("Ako email postoji, poslan je link za reset lozinke.");
        // }

        [AllowAnonymous]
        [HttpPost]
        public override Task<UserResponse> Create([FromBody] UserInsertRequest request)
        {
            return base.Create(request);
        }
    }
}
