using PrisonManagementSystem.BL.DTOs.Incident;
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
    public interface IIncidentService
    {
        Task<GenericResponseModel<bool>> CreateIncidentAsync(CreateIncidentDto dto);
        Task<GenericResponseModel<bool>> UpdateIncidentAsync(Guid incidentId, UpdateIncidentDto dto);
        Task<GenericResponseModel<GetIncidentDto>> GetIncidentByIdAsync(Guid incidentId);
        Task<GenericResponseModel<PaginationResponse<GetIncidentDto>>> GetAllIncidentsAsync(PaginationRequest paginationRequest);
        Task<GenericResponseModel<bool>> DeleteIncidentAsync(Guid incidentId, bool isHardDelete);
    }
}
