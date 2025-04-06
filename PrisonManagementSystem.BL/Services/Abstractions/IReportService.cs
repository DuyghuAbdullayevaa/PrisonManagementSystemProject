using PrisonManagementSystem.BL.DTOs.RequestFeedback;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface IReportService
    {
       
        Task<GenericResponseModel<PaginationResponse<GetReportDto>>> GetAllReportsAsync(PaginationRequest paginationRequest);

        Task<GenericResponseModel<GetReportDto>> GetReportByIdAsync(Guid reportId);

        Task<GenericResponseModel<bool>> AddReportToIncidentAsync(Guid incidentId, CreateReportDto reportDto);

        Task<GenericResponseModel<bool>> UpdateReportAsync(Guid reportId, UpdateReportDto updateReportDto);

        Task<GenericResponseModel<bool>> DeleteReportAsync(Guid reportId, bool isHardDelete);
    }
}
