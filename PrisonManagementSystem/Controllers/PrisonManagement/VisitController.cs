using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Visit;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/visits")]
    public class VisitController : BaseController
    {
        private readonly IVisitService _visitService;

        public VisitController(IVisitService visitService) =>
            _visitService = visitService;

        [Authorize(Roles = "Admin,Warden,Guard,Clerk")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _visitService.GetAllVisitsAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden,Guard,Visitor,Clerk")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _visitService.GetVisitByIdAsync(id));

        [Authorize(Roles = "Admin,Warden,Visitor,Clerk")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateVisitDto dto) =>
            CreateResponse(await _visitService.CreateVisitAsync(dto));

        [Authorize(Roles = "Admin,Warden,Clerk")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateVisitDto dto) =>
            CreateResponse(await _visitService.UpdateVisitAsync(id, dto));

        [Authorize(Roles = "Admin,Warden,Clerk")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _visitService.DeleteVisitAsync(id, isHardDelete));
    }
}
