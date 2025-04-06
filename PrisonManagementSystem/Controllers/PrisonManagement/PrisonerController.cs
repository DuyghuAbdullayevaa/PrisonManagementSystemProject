using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/prisoners")]
    public class PrisonerController : BaseController
    {
        private readonly IPrisonerService _prisonerService;

        public PrisonerController(IPrisonerService prisonerService) => _prisonerService = prisonerService;


        [Authorize(Roles = "Admin,Warden")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _prisonerService.GetAllPrisonersAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _prisonerService.GetPrisonerByIdAsync(id));

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreatePrisonerDto dto) =>
            CreateResponse(await _prisonerService.AddPrisonerAsync(dto));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdatePrisonerDto dto) =>
            CreateResponse(await _prisonerService.UpdatePrisonerAsync(id, dto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _prisonerService.DeletePrisonerAsync(id, isHardDelete));

    }
}