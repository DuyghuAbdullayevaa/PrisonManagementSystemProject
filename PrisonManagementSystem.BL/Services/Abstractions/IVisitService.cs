using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface IVisitService
    {
        Task<GenericResponseModel<GetVisitDto>> GetVisitByIdAsync(Guid id);
        Task<GenericResponseModel<PaginationResponse<GetVisitDto>>> GetAllVisitsAsync(PaginationRequest paginationRequest);
        Task<GenericResponseModel<GetVisitDto>> CreateVisitAsync(CreateVisitDto createVisitDto);
        Task<GenericResponseModel<bool>> UpdateVisitAsync(Guid id, UpdateVisitDto updateVisitDto);
        Task<GenericResponseModel<bool>> DeleteVisitAsync(Guid id, bool isHardDelete);
    }
}
