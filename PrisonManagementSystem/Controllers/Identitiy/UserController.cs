using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Identiity.User;
using PrisonManagementSystem.BL.Services.Abstractions.Identity;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers.Identity
{
    [Route("api/v1/users")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) =>
            _userService = userService;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateAsync([FromBody] CreateUserDto dto) =>
            CreateResponse(await _userService.CreateUserAsync(dto));

        [HttpGet]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<ActionResult> GetAllAsync() =>
            CreateResponse(await _userService.GetAllUsersAsync());
  
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Warden")]
        public async Task<ActionResult> GetByIdAsync(string id) =>
            CreateResponse(await _userService.GetUserByIdAsync(id));

           [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteAsync(string id) =>
            CreateResponse(await _userService.DeleteUserAsync(id));

        [HttpPost("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignRolesAsync(string id, [FromBody] string[] roles) =>
            CreateResponse(await _userService.AssignRoleToUserAsync(id, roles));

        [HttpPost("create-staff-user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateStaffUserAsync([FromBody] RegisterStaffUserDto registerStaffUserDto) =>
            CreateResponse(await _userService.CreateStaffUserAsync(registerStaffUserDto));

        [HttpPost("create-visitor-user")]
        [Authorize(Roles = "Admin,Staff,Warden")] 
        public async Task<ActionResult> RegisterVisitorWithUserAsync(RegisterVisitorUserDto dto) =>
             CreateResponse(await _userService.RegisterVisitorWithUserAsync(dto));
    }
}
