
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface IPrisonerService
    {
        Task<GenericResponseModel<PaginationResponse<GetPrisonerDto>>> GetAllPrisonersAsync(PaginationRequest request);
        Task<GenericResponseModel<GetPrisonerDto>> GetPrisonerByIdAsync(Guid id); 
        Task<GenericResponseModel<bool>> AddPrisonerAsync(CreatePrisonerDto dto);

        Task<GenericResponseModel<GetPrisonerDto>> UpdatePrisonerAsync(Guid id, UpdatePrisonerDto dto); 
        Task<GenericResponseModel<bool>> DeletePrisonerAsync(Guid id, bool isHardDelete);
       
    }
}
