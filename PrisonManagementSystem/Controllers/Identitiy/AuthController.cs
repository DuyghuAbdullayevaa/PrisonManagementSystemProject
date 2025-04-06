using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.DTOs.Identiity.Token;
using PrisonManagementSystem.BL.Services.Abstractions.Identity;
using System.Threading.Tasks;
using PrisonManagementSystem.API.Controllers.Base;

namespace PrisonManagementSystem.API.Controllers.Identitiy
{
    [Route("api/v1/auths")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authoService;

        public AuthController(IAuthService authoService) => 
            _authoService = authoService;

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> LoginAsync(string userNameOrEmail, string password) =>
            CreateResponse(await _authoService.LoginAsync(userNameOrEmail, password));

        [HttpPost("refresh")]
        [Authorize]
        public async Task<ActionResult> RefreshAsync(TokenApiDto tokenApiDto) =>
            CreateResponse(await _authoService.RefreshAsync(tokenApiDto));

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> LogOutAsync([FromBody] string userNameOrEmail) =>
            CreateResponse(await _authoService.LogOutAsync(userNameOrEmail));

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPasswordAsync(string email, string newPassword) =>
            CreateResponse(await _authoService.ResetPasswordAsync(email, newPassword));

        [HttpPost("change-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePasswordAsync(string userId, string oldPassword, string newPassword) =>
             CreateResponse(await _authoService.ChangeUserPasswordAsync(userId, oldPassword, newPassword));

        [HttpPost("revoke")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RevokeAsync() =>
            CreateResponse(await _authoService.RevokeAsync());
    }
}
