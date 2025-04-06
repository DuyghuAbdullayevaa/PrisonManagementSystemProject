using PrisonManagementSystem.BL.DTOs.Cell;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface ICellService
    {
        Task<GenericResponseModel<PaginationResponse<GetCellDto>>> GetAllCellsAsync(PaginationRequest paginationRequest);
        Task<GenericResponseModel<CreateCellDto>> AddCellAsync(CreateCellDto createCellDto);
        Task<GenericResponseModel<bool>> UpdateCellAsync(Guid cellId, UpdateCellDto updateCellDto);
        Task<GenericResponseModel<bool>> DeleteCellAsync(Guid cellId, bool isHardDelete);
        Task<GenericResponseModel<GetCellDto>> GetCellByIdAsync(Guid cellId);
        Task<GenericResponseModel<IEnumerable<GetCellDto>>> GetAllCellsByPrisonIdAsync(Guid prisonId);
    }
}
