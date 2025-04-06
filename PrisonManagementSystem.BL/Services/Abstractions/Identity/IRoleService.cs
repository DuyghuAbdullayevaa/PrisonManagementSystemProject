using PrisonManagementSystem.BL.DTOs.Identiity.Role;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions.Identity
{
    public interface IRoleService
    {
        Task<GenericResponseModel<bool>> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<GenericResponseModel<bool>> DeleteRoleAsync(string id);
        Task<GenericResponseModel<GetRoleDto>> GetRoleByIdAsync(string id);
        Task<GenericResponseModel<bool>> UpdateRoleAsync(string id, UpdateRoleDto updateRoleDto);
        Task<GenericResponseModel<List<GetRoleDto>>> GetAllRolesAsync();
        Task<GenericResponseModel<GetRoleDto[]>> GetRolesToUserAsync(string userIdOrName);
    }
}