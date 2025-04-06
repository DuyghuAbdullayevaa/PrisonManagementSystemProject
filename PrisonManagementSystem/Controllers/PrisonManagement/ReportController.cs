using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.RequestFeedback;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/reports")]
    [Authorize(Roles = "Admin,Warden")]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService) =>
            _reportService = reportService;

        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _reportService.GetAllReportsAsync(paginationRequest));

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
        CreateResponse(await _reportService.GetReportByIdAsync(id));

        [HttpPost("incident/{incidentId}")]
        public async Task<ActionResult> CreateAsync(Guid incidentId, [FromBody] CreateReportDto dto) =>
            CreateResponse(await _reportService.AddReportToIncidentAsync(incidentId, dto));

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateReportDto dto) =>
         CreateResponse(await _reportService.UpdateReportAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _reportService.DeleteReportAsync(id, isHardDelete));
    }
}
