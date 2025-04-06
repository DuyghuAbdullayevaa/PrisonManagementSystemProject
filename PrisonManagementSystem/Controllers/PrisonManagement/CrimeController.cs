using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrisonManagementSystem.API.Controllers.Base;
using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Threading.Tasks;

namespace PrisonManagementSystem.API.Controllers
{

    [Route("api/v1/crimes")]
    [Authorize(Roles = "Admin,Warden")]
    public class CrimeController : BaseController
    {
        private readonly ICrimeService _crimeService;

        public CrimeController(ICrimeService crimeService) => _crimeService = crimeService;

        [HttpGet]
        public async Task<ActionResult> GetAllAsync([FromQuery] PaginationRequest paginationRequest) =>
            CreateResponse(await _crimeService.GetAllCrimesAsync(paginationRequest));

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(Guid id) =>
            CreateResponse(await _crimeService.GetCrimeByIdAsync(id));

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateCrimeDto createCrimeDto) =>
            CreateResponse(await _crimeService.CreateCrimeAsync(createCrimeDto));

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateCrimeDto dto) =>
            CreateResponse(await _crimeService.UpdateCrimeAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, bool isHardDelete) =>
            CreateResponse(await _crimeService.DeleteCrimeAsync(id, isHardDelete));
    }
}
