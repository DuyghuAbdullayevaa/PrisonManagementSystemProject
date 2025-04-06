using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Visitor;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/visitors")]
    public class VisitorController : BaseController
    {
        private readonly IVisitorService _visitorService;

        public VisitorController(IVisitorService visitorService) =>
            _visitorService = visitorService;

        [Authorize(Roles = "Admin,Warden,Clerk")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _visitorService.GetAllVisitorsAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden,Visitor,Clerk")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _visitorService.GetVisitorByIdAsync(id));

        [Authorize(Roles = "Admin,Warden,Clerk")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateVisitorDto dto) =>
            CreateResponse(await _visitorService.CreateVisitorAsync(dto));

        [Authorize(Roles = "Admin,Warden,Clerk")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateVisitorDto dto) =>
            CreateResponse(await _visitorService.UpdateVisitorAsync(id, dto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _visitorService.DeleteVisitorAsync(id, isHardDelete));
    }
}
