using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.DTOs.Visitor;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface IVisitorService
    {
        Task<GenericResponseModel<GetVisitorDto>> GetVisitorByIdAsync(Guid id);
        Task<GenericResponseModel<PaginationResponse<GetVisitorDto>>> GetAllVisitorsAsync(PaginationRequest paginationRequest);
        Task<GenericResponseModel<bool>> CreateVisitorAsync(CreateVisitorDto dto);
        Task<GenericResponseModel<bool>> UpdateVisitorAsync(Guid id, UpdateVisitorDto updateVisitorDto);
        Task<GenericResponseModel<bool>> DeleteVisitorAsync(Guid id, bool isHardDelete);
    }
}
