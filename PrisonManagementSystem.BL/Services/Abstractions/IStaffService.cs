using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.DTOs.Staff;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface IStaffService
    {
        Task<GenericResponseModel<PaginationResponse<GetStaffDto>>> GetAllStaffAsync(PaginationRequest paginationRequest);
        Task<GenericResponseModel<bool>> CreateStaffAsync(CreateStaffDto createStaffDto);
        Task<GenericResponseModel<GetStaffDto>> GetStaffByIdAsync(Guid id);
        Task<GenericResponseModel<bool>> UpdateStaffAsync(Guid id, UpdateStaffDto updateStaffDto);
        Task<GenericResponseModel<bool>> DeleteStaffAsync(Guid id, bool isHardDelete);
    }
}
