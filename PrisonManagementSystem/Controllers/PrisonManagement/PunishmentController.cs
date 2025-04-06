using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Punishment;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/punishments")]

    public class PunishmentController : BaseController
    {
        private readonly IPunishmentService _punishmentService;

        public PunishmentController(IPunishmentService punishmentService) =>
            _punishmentService = punishmentService;

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _punishmentService.GetAllPunishmentsAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _punishmentService.GetPunishmentByIdAsync(id));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreatePunishmentDto dto) =>
            CreateResponse(await _punishmentService.CreatePunishmentAsync(dto));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdatePunishmentDto dto) =>
            CreateResponse(await _punishmentService.UpdatePunishmentAsync(id, dto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _punishmentService.DeletePunishmentAsync(id, isHardDelete));
    }
}