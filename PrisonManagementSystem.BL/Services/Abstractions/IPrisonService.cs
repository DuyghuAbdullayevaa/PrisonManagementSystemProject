using PrisonManagementSystem.BL.DTOs.Prison;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface IPrisonService
    {
        Task<GenericResponseModel<PaginationResponse<GetPrisonDto>>> GetAllPrisonsAsync(PaginationRequest paginationRequest);
        Task<GenericResponseModel<GetPrisonDto>> GetPrisonByIdAsync(Guid id);
        Task<GenericResponseModel<bool>> CreatePrisonAsync(CreatePrisonDto prisonCreateDto);
        Task<GenericResponseModel<bool>> UpdatePrisonAsync(Guid id, UpdatePrisonDto prisonUpdateDto);
        Task<GenericResponseModel<bool>> DeletePrisonAsync(Guid id, bool isHardDelete);
        Task<GenericResponseModel<bool>> ChangePrisonStatusAsync(Guid id, bool isActive);



    }
}
