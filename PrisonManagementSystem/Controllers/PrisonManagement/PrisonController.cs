using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Prison;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/prisons")]
    public class PrisonController : BaseController
    {
        private readonly IPrisonService _prisonService;

        public PrisonController(IPrisonService prisonService) => _prisonService = prisonService;

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _prisonService.GetAllPrisonsAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _prisonService.GetPrisonByIdAsync(id));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreatePrisonDto dto) =>
            CreateResponse(await _prisonService.CreatePrisonAsync(dto));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdatePrisonDto dto) =>
            CreateResponse(await _prisonService.UpdatePrisonAsync(id, dto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _prisonService.DeletePrisonAsync(id, isHardDelete));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPatch("change-status/{id}")]
        public async Task<ActionResult> ChangeStatus(Guid id, [FromQuery] bool isActive) =>
            CreateResponse(await _prisonService.ChangePrisonStatusAsync(id, isActive));
    }
}
