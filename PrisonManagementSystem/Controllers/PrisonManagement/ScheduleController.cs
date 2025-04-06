using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/schedules")]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService) =>
            _scheduleService = scheduleService;

        [Authorize(Roles = "Admin,Warden,Guard,Clerk")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _scheduleService.GetAllSchedulesAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden,Guard,Clerk")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _scheduleService.GetScheduleByIdAsync(id));

        [Authorize(Roles = "Admin,Warden,Clerk")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateScheduleDto dto) =>
            CreateResponse(await _scheduleService.AssignScheduleToStaffAsync(dto));

        [Authorize(Roles = "Admin,Warden,Clerk")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateScheduleDto dto) =>
            CreateResponse(await _scheduleService.UpdateScheduleAsync(id, dto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _scheduleService.DeleteScheduleAsync(id, isHardDelete));
    }
}
