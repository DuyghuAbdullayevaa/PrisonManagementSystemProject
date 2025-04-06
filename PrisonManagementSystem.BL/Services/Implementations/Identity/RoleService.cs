using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PrisonManagementSystem.BL.DTOs.Identiity.Role;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Services.Abstractions.Identity;
using PrisonManagementSystem.DAL.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Implementations.Identity
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleService(IMapper mapper, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<GenericResponseModel<bool>> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            if (string.IsNullOrWhiteSpace(createRoleDto.Name))
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Role name is required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var result = await _roleManager.CreateAsync(new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = createRoleDto.Name
            });

            if (!result.Succeeded)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    string.Join(", ", result.Errors.Select(e => e.Description)),
                    (int)HttpStatusCode.BadRequest);
            }

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                (int)HttpStatusCode.Created,
                "Role created successfully.");
        }


        public async Task<GenericResponseModel<bool>> DeleteRoleAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Role ID is required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Role not found.",
                    (int)HttpStatusCode.NotFound);
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    string.Join(", ", result.Errors.Select(e => e.Description)),
                    (int)HttpStatusCode.BadRequest);
            }

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                (int)HttpStatusCode.OK,
                "Role deleted successfully.");
        }

        public async Task<GenericResponseModel<GetRoleDto>> GetRoleByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return GenericResponseModel<GetRoleDto>.FailureResponse(
                    "Role ID is required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return GenericResponseModel<GetRoleDto>.FailureResponse(
                    "Role not found.",
                    (int)HttpStatusCode.NotFound);
            }

            var roleDto = _mapper.Map<GetRoleDto>(role);
            return GenericResponseModel<GetRoleDto>.SuccessResponse(
                roleDto,
                (int)HttpStatusCode.OK,
                "Role retrieved successfully.");
        }

        public async Task<GenericResponseModel<bool>> UpdateRoleAsync(string id, UpdateRoleDto updateRoleDto)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(updateRoleDto.Name))
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Role ID and name are required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Role not found.",
                    (int)HttpStatusCode.NotFound);
            }

            role.Name = updateRoleDto.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    string.Join(", ", result.Errors.Select(e => e.Description)),
                    (int)HttpStatusCode.BadRequest);
            }

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                (int)HttpStatusCode.OK,
                "Role updated successfully.");
        }

        public async Task<GenericResponseModel<List<GetRoleDto>>> GetAllRolesAsync()
        {
            var roles = _roleManager.Roles.ToList();
            var rolesDto = _mapper.Map<List<GetRoleDto>>(roles);

            return GenericResponseModel<List<GetRoleDto>>.SuccessResponse(
                rolesDto,
                (int)HttpStatusCode.OK,
                roles.Any() ? "Roles retrieved successfully." : "No roles found.");
        }

        public async Task<GenericResponseModel<GetRoleDto[]>> GetRolesToUserAsync(string userIdOrName)
        {
            if (string.IsNullOrWhiteSpace(userIdOrName))
            {
                return GenericResponseModel<GetRoleDto[]>.FailureResponse(
                    "User ID or Name is required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var user = await _userManager.FindByIdAsync(userIdOrName) ??
                      await _userManager.FindByNameAsync(userIdOrName);

            if (user == null)
            {
                return GenericResponseModel<GetRoleDto[]>.FailureResponse(
                    "User not found.",
                    (int)HttpStatusCode.NotFound);
            }

            var roleNames = await _userManager.GetRolesAsync(user);
            var roleDtos = new List<GetRoleDto>();

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roleDtos.Add(_mapper.Map<GetRoleDto>(role));
                }
            }

            return GenericResponseModel<GetRoleDto[]>.SuccessResponse(
                roleDtos.ToArray(),
                (int)HttpStatusCode.OK,
                "Roles retrieved successfully.");
        }
    }
}