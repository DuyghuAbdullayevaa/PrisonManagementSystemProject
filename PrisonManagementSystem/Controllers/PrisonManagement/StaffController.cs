using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Staff;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/staffs")]
    public class StaffController : BaseController
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService) =>
            _staffService = staffService;

        [Authorize(Roles = "Admin,Warden")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _staffService.GetAllStaffAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden,Guard,Clerk")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _staffService.GetStaffByIdAsync(id));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateStaffDto dto) =>
            CreateResponse(await _staffService.CreateStaffAsync(dto));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateStaffDto dto) =>
            CreateResponse(await _staffService.UpdateStaffAsync(id, dto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _staffService.DeleteStaffAsync(id, isHardDelete));
    }
}
