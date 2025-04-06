using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.DTOs.Punishment;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.BL.Services.Abstractions
{
    public interface IPunishmentService
    {
        Task<GenericResponseModel<bool>> CreatePunishmentAsync(CreatePunishmentDto createPunishmentDto);
        Task<GenericResponseModel<bool>> UpdatePunishmentAsync(Guid punishmentId, UpdatePunishmentDto updatePunishmentDto);
        Task<GenericResponseModel<GetPunishmentDto>> GetPunishmentByIdAsync(Guid punishmentId);
        Task<GenericResponseModel<PaginationResponse<GetPunishmentDto>>> GetAllPunishmentsAsync(PaginationRequest paginationRequest);
        Task<GenericResponseModel<bool>> DeletePunishmentAsync(Guid punishmentId, bool isHardDelete);
    }
}
