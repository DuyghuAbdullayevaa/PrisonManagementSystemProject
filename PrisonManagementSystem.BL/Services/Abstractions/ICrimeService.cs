using PrisonManagementSystem.BL.DTOs.Crime;

using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface ICrimeService
    {
        Task<GenericResponseModel<CreateCrimeDto>> CreateCrimeAsync(CreateCrimeDto createCrimeDto);
        Task<GenericResponseModel<UpdateCrimeDto>> UpdateCrimeAsync(Guid crimeId, UpdateCrimeDto updateCrimeDto);
        Task<GenericResponseModel<GetCrimeDto>> GetCrimeByIdAsync(Guid crimeId);
        Task<GenericResponseModel<bool>> DeleteCrimeAsync(Guid crimeId, bool isHardDelete);
        Task<GenericResponseModel<PaginationResponse<GetCrimeDto>>> GetAllCrimesAsync(PaginationRequest paginationRequest);
    }
}
