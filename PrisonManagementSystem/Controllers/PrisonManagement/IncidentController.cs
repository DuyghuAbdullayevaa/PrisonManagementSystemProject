using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/incidents")]
    public class IncidentController : BaseController
    {
        private readonly IIncidentService _incidentService;

        public IncidentController(IIncidentService incidentService) => _incidentService = incidentService;

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _incidentService.GetAllIncidentsAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _incidentService.GetIncidentByIdAsync(id));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateIncidentDto dto) =>
            CreateResponse(await _incidentService.CreateIncidentAsync(dto));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateIncidentDto dto) =>
            CreateResponse(await _incidentService.UpdateIncidentAsync(id, dto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _incidentService.DeleteIncidentAsync(id, isHardDelete));
    }
}
