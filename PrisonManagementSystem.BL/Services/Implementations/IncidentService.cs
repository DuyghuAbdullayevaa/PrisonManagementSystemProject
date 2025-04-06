using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IReportRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerIncidentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentCellRepository;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Prison;
using Serilog;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class IncidentService : IIncidentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIncidentReadRepository _incidentReadRepository;
        private readonly IIncidentWriteRepository _incidentWriteRepository;
        private readonly IPrisonReadRepository _prisonReadRepository;
        private readonly IPrisonerReadRepository _prisonerReadRepository;
        private readonly ICellReadRepository _cellReadRepository;
        private readonly IReportWriteRepository _reportWriteRepository;
        private readonly IPrisonerIncidentWriteRepository _prisonerIncidentWriteRepository;
        private readonly IIncidentCellWriteRepository _incidentCellWriteRepository;


        public IncidentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _incidentReadRepository = _unitOfWork.GetRepository<IIncidentReadRepository>();
            _incidentWriteRepository = _unitOfWork.GetRepository<IIncidentWriteRepository>();
            _prisonReadRepository = _unitOfWork.GetRepository<IPrisonReadRepository>();
            _prisonerReadRepository = _unitOfWork.GetRepository<IPrisonerReadRepository>();
            _cellReadRepository = _unitOfWork.GetRepository<ICellReadRepository>();
            _reportWriteRepository = _unitOfWork.GetRepository<IReportWriteRepository>();
            _prisonerIncidentWriteRepository = _unitOfWork.GetRepository<IPrisonerIncidentWriteRepository>();
            _incidentCellWriteRepository = _unitOfWork.GetRepository<IIncidentCellWriteRepository>();

        }
        public async Task<GenericResponseModel<bool>> CreateIncidentAsync(CreateIncidentDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. Check if the prison exists
                var prison = await _prisonReadRepository.GetByIdAsync(dto.PrisonId);
                if (prison == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Invalid PrisonId. Prison not found.",
                        400
                    );
                }

                // 2. Validate prisoners belong to the prison
                var validPrisoners = await _prisonerReadRepository.GetAllAsync(
                    p => dto.PrisonerIds.Contains(p.Id) && p.Cell != null && p.Cell.PrisonId == dto.PrisonId,
                    include: query => query.Include(p => p.Cell)
                );

                if (validPrisoners.Count != dto.PrisonerIds.Count)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "One or more PrisonerIds are invalid or do not belong to the given prison.",
                        400
                    );
                }

                // 3. Validate cells belong to the prison
                if (dto.CellIds != null && dto.CellIds.Any())
                {
                    var validCells = await _cellReadRepository.GetAllAsync(
                        c => dto.CellIds.Contains(c.Id) && c.PrisonId == dto.PrisonId
                    );

                    if (validCells.Count != dto.CellIds.Count)
                    {
                        return GenericResponseModel<bool>.FailureResponse(
                            "One or more CellIds are invalid or do not belong to the given prison.",
                            400
                        );
                    }
                }

                // 4. Create and add incident
                var incident = _mapper.Map<Incident>(dto);
                incident.Status = dto.Status;  // Enum value is set from DTO
                incident.IncidentDate = dto.IncidentDate;
                incident.Type = dto.Type;  // Enum value is set from DTO

                await _incidentWriteRepository.AddAsync(incident);


                // 5. Create and add report
                var report = new Report
                {
                    ReportType = ReportType.Incident,
                    RelatedIncidentId = incident.Id,
                    Descriptions = incident.Description
                };
                await _reportWriteRepository.AddAsync(report);

                // 6. Add cell links if any
                if (dto.CellIds != null && dto.CellIds.Any())
                {
                    var incidentCellLinks = dto.CellIds.Select(cellId => new IncidentCell
                    {
                        IncidentId = incident.Id,
                        CellId = cellId
                    }).ToList();

                    await _incidentCellWriteRepository.AddRangeAsync(incidentCellLinks);
                }

                // 7. Add prisoner links
                var prisonerIncidents = dto.PrisonerIds.Select(prisonerId => new PrisonerIncident
                {
                    PrisonerId = prisonerId,
                    IncidentId = incident.Id
                }).ToList();

                await _prisonerIncidentWriteRepository.AddRangeAsync(prisonerIncidents);
                await _unitOfWork.CommitAsync(); // Commit after adding the prisoner links

                // Commit the entire transaction
                await _unitOfWork.CommitTransactionAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    true,
                    201,
                    "Incident and Report created successfully, pending investigation."
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while creating the incident.");

                await _unitOfWork.RollbackTransactionAsync(); // Rollback transaction in case of failure
                return GenericResponseModel<bool>.FailureResponse(
                    $"An error occurred while creating the incident: {ex.Message}",
                    500
                );
            }
        }

        public async Task<GenericResponseModel<bool>> UpdateIncidentAsync(Guid id, UpdateIncidentDto updateIncidentDto)
        {
            var incident = await _incidentReadRepository.GetSingleAsync(
                      predicate: i => i.Id == id,
                      include: query => query
                          .Include(i => i.PrisonerIncidents)
                          .Include(i => i.IncidentCells)
                  );

            if (incident == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Incident not found.",
                    404
                );
            }

            _mapper.Map(updateIncidentDto, incident);

            // Update Prisoners
            if (updateIncidentDto.PrisonerIds != null)
            {
                var validPrisoners = await _prisonerReadRepository.GetAllAsync(
                    p => updateIncidentDto.PrisonerIds.Contains(p.Id) && p.Cell != null,
                    include: query => query.Include(p => p.Cell)
                );

                if (validPrisoners.Count != updateIncidentDto.PrisonerIds.Count)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "One or more PrisonerIds are invalid.",
                        400
                    );
                }

                await _prisonerIncidentWriteRepository.RemoveRangeAsync(incident.PrisonerIncidents);
                var prisonerIncidentsToAdd = updateIncidentDto.PrisonerIds
                    .Select(prisonerId => new PrisonerIncident
                    {
                        PrisonerId = prisonerId,
                        IncidentId = incident.Id
                    })
                    .ToList();

                await _prisonerIncidentWriteRepository.AddRangeAsync(prisonerIncidentsToAdd);
            }

            // Update Cells
            if (updateIncidentDto.CellIds != null)
            {
                await _incidentCellWriteRepository.RemoveRangeAsync(incident.IncidentCells);
                var incidentCellsToAdd = updateIncidentDto.CellIds
                    .Select(cellId => new IncidentCell
                    {
                        IncidentId = incident.Id,
                        CellId = cellId
                    })
                    .ToList();

                await _incidentCellWriteRepository.AddRangeAsync(incidentCellsToAdd);
            }

            await _incidentWriteRepository.UpdateAsync(incident);
            await _unitOfWork.CommitAsync();

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                200,
                "Incident updated successfully."
            );

        }

        public async Task<GenericResponseModel<PaginationResponse<GetIncidentDto>>> GetAllIncidentsAsync(PaginationRequest paginationRequest)
        {

            var incidents = await _incidentReadRepository.GetAllByPagingAsync(
                    predicate: null,
                    include: q => q
                        .Include(i => i.Prison)
                        .Include(i => i.PrisonerIncidents).ThenInclude(pi => pi.Prisoner)
                        .Include(i => i.IncidentCells).ThenInclude(ic => ic.Cell)
                        .Include(i => i.IncidentPunishments),
                    orderBy: q => q.OrderBy(i => i.IncidentDate),
                    currentPage: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize
                );

            int totalCount = await _incidentReadRepository.GetCountAsync();
            var incidentDtos = _mapper.Map<List<GetIncidentDto>>(incidents);

            var paginatedResponse = new PaginationResponse<GetIncidentDto>(
                totalCount,
                incidentDtos,
                paginationRequest.PageNumber,
                paginationRequest.PageSize
            );

            return GenericResponseModel<PaginationResponse<GetIncidentDto>>.SuccessResponse(
                paginatedResponse,
                200,
                "Incidents retrieved successfully."
            );

        }

        public async Task<GenericResponseModel<GetIncidentDto>> GetIncidentByIdAsync(Guid id)
        {

            var incident = await _incidentReadRepository.GetSingleAsync(
                predicate: i => i.Id == id,
                include: q => q
                    .Include(i => i.Prison)
                    .Include(i => i.PrisonerIncidents).ThenInclude(pi => pi.Prisoner)
                    .Include(i => i.IncidentCells).ThenInclude(ic => ic.Cell)
                    .Include(i => i.IncidentPunishments)
            );

            if (incident == null)
            {
                return GenericResponseModel<GetIncidentDto>.FailureResponse(
                    "Incident not found.",
                    404
                );
            }

            var incidentDto = _mapper.Map<GetIncidentDto>(incident);
            return GenericResponseModel<GetIncidentDto>.SuccessResponse(
                incidentDto,
                200,
                "Incident retrieved successfully."
            );

        }

        public async Task<GenericResponseModel<bool>> DeleteIncidentAsync(Guid id, bool isHardDelete)
        {

            var incident = await _incidentReadRepository.GetByIdAsync(id);
            if (incident == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Incident not found.",
                    404
                );
            }

            if (incident.IncidentDate < DateTime.UtcNow.AddMonths(-1))
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Old incidents cannot be deleted.",
                    400
                );
            }

            var result = await _incidentWriteRepository.DeleteAsync(incident, isHardDelete);
            if (!result)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Error occurred while deleting the incident.",
                    400
                );
            }

            await _unitOfWork.CommitAsync();
            return GenericResponseModel<bool>.SuccessResponse(
                true,
                200,
                "Incident deleted successfully"
            );

        }
    }
}