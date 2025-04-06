using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Identiity.Role;
using PrisonManagementSystem.BL.Services.Abstractions.Identity;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers.Identity
{
    [Route("api/v1/roles")]
    [Authorize(Roles = "Admin")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService) =>
            _roleService = roleService;

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateRoleDto dto) =>
            CreateResponse(await _roleService.CreateRoleAsync(dto));

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id) =>
            CreateResponse(await _roleService.DeleteRoleAsync(id));

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(string id) =>
            CreateResponse(await _roleService.GetRoleByIdAsync(id));

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(string id, [FromBody] UpdateRoleDto dto) =>
            CreateResponse(await _roleService.UpdateRoleAsync(id, dto));

        [HttpGet]
        public async Task<ActionResult> GetAllAsync() =>
            CreateResponse(await _roleService.GetAllRolesAsync());

        [HttpGet("user/{userId}")]
        public async Task<ActionResult> GetByUserIdAsync(string userId) =>
            CreateResponse(await _roleService.GetRolesToUserAsync(userId));
    }
}
