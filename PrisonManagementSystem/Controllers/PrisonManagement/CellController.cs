using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Cell;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{
    [Route("api/v1/cells")]
    public class CellController : BaseController
    {
        private readonly ICellService _cellService;

        public CellController(ICellService cellService) => _cellService = cellService;

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _cellService.GetAllCellsAsync(paginationRequest));

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _cellService.GetCellByIdAsync(id));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateCellDto dto) =>
            CreateResponse(await _cellService.AddCellAsync(dto));

        [Authorize(Roles = "Admin,Warden")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateCellDto dto) =>
            CreateResponse(await _cellService.UpdateCellAsync(id, dto));

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _cellService.DeleteCellAsync(id, isHardDelete));

        [Authorize(Roles = "Admin,Warden,Guard")]
        [HttpGet("prison/{prisonId}")]
        public async Task<ActionResult> GetAllByPrisonAsync(Guid prisonId) =>
            CreateResponse(await _cellService.GetAllCellsByPrisonIdAsync(prisonId));
    }
}
