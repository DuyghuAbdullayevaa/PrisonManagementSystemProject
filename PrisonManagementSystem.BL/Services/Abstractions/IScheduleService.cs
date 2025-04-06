using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface IScheduleService
    {
        Task<GenericResponseModel<PaginationResponse<GetScheduleDto>>> GetAllSchedulesAsync(PaginationRequest paginationRequest);
        Task<GenericResponseModel<bool>> AssignScheduleToStaffAsync(CreateScheduleDto createScheduleDto);
        Task<GenericResponseModel<GetScheduleDto>> GetScheduleByIdAsync(Guid id);
        Task<GenericResponseModel<bool>> UpdateScheduleAsync(Guid id, UpdateScheduleDto updateScheduleDto);
        Task<GenericResponseModel<bool>> DeleteScheduleAsync(Guid id, bool isHardDelete);
    }
}
