using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.BL.DTOs.RequestFeedback;

using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IReportRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.ReportRepository;
using PrisonManagementSystem.BL.Extensions;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IReportReadRepository _reportReadRepository;
        private readonly IReportWriteRepository _reportWriteRepository;
        private readonly IIncidentReadRepository _incidentReadRepository;


        public ReportService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            _reportReadRepository = _unitOfWork.GetRepository<IReportReadRepository>();
            _reportWriteRepository = _unitOfWork.GetRepository<IReportWriteRepository>();
            _incidentReadRepository = _unitOfWork.GetRepository<IIncidentReadRepository>();
        }



        public async Task<GenericResponseModel<PaginationResponse<GetReportDto>>> GetAllReportsAsync(PaginationRequest paginationRequest)
        {
            var reports = await _reportReadRepository.GetAllByPagingAsync(
                    include: query => query
                        .Include(r => r.RelatedIncident)
                        .ThenInclude(i => i.Prison)
                        .Include(r => r.RelatedIncident)
                        .ThenInclude(i => i.PrisonerIncidents)
                        .ThenInclude(pi => pi.Prisoner)
                        .Include(r => r.RelatedIncident)
                        .ThenInclude(i => i.IncidentCells)
                        .ThenInclude(ic => ic.Cell),
                    orderBy: q => q.OrderByDescending(r => r.CreateDate),
                    currentPage: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize
                );

            if (!reports.Any())
            {
                return GenericResponseModel<PaginationResponse<GetReportDto>>.SuccessResponse(
                    data: new PaginationResponse<GetReportDto>(0, new List<GetReportDto>(), paginationRequest.PageNumber, paginationRequest.PageSize),
                    statusCode: 200,
                    message: "No reports found"
                );
            }

            int totalCount = await _reportReadRepository.GetCountAsync();

            // Extension method istifadə edərək mappingi həyata keçiririk
            var reportDtos = reports.Select(report => report.ToGetReportDto()).ToList();

            return GenericResponseModel<PaginationResponse<GetReportDto>>.SuccessResponse(
                data: new PaginationResponse<GetReportDto>(totalCount, reportDtos, paginationRequest.PageNumber, paginationRequest.PageSize),
                statusCode: 200,
                message: "Reports retrieved successfully"
            );
            ;

        }



        public async Task<GenericResponseModel<GetReportDto>> GetReportByIdAsync(Guid reportId)
        {
           
                var report = await _reportReadRepository.GetSingleAsync(
                    r => r.Id == reportId,
                    include: query => query
                        .Include(r => r.RelatedIncident)
                            .ThenInclude(i => i.Prison) // PrisonName almaq üçün
                        .Include(r => r.RelatedIncident)
                            .ThenInclude(i => i.PrisonerIncidents)
                                .ThenInclude(pi => pi.Prisoner) // Prisoner adları üçün
                        .Include(r => r.RelatedIncident)
                            .ThenInclude(i => i.IncidentCells)
                                .ThenInclude(ic => ic.Cell) // Cell adları üçün
                );

                if (report == null)
                {
                    return GenericResponseModel<GetReportDto>.FailureResponse(
                        error: "Report not found",
                        statusCode: 404
                    );
                }

                // Extension metodundan istifadə edirik
                var reportDto = report.ToGetReportDto();

                return GenericResponseModel<GetReportDto>.SuccessResponse(
                    data: reportDto,
                    statusCode: 200,
                    message: "Report retrieved successfully"
                );
           
        }

        public async Task<GenericResponseModel<bool>> AddReportToIncidentAsync(Guid incidentId, CreateReportDto reportDto)
        {
            await _unitOfWork.BeginTransactionAsync(); // Transaction-u UnitOfWork üzərindən başlatırıq
           
                var incident = await _incidentReadRepository.GetByIdAsync(incidentId);
                if (incident == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Incident not found",
                        statusCode: 404
                    );
                }

                if (incident.Status == IncidentStatus.Resolved || incident.Status == IncidentStatus.Dismissed)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Cannot add reports to resolved or dismissed incidents",
                        statusCode: 400
                    );
                }

                var report = _mapper.Map<Report>(reportDto);
                report.ReportType = reportDto.ReportType; // DTO-dan ReportType təyin edilir
                report.RelatedIncidentId = incidentId;

                await _reportWriteRepository.AddAsync(report);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync(); // Transaction-u uğurla tamamlayırıq

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 201,
                    message: "Report added successfully to incident"
                );
           
        }


        public async Task<GenericResponseModel<bool>> UpdateReportAsync(Guid reportId, UpdateReportDto updateReportDto)
        {
           
                var report = await _reportReadRepository.GetByIdAsync(reportId);
                if (report == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Report not found",
                        statusCode: 404
                    );
                }

                _mapper.Map(updateReportDto, report);
                report.ReportType = updateReportDto.ReportType; // Use the ReportType from the DTO

                await _reportWriteRepository.UpdateAsync(report);
                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: "Report updated successfully"
                );
          
        }


        public async Task<GenericResponseModel<bool>> DeleteReportAsync(Guid reportId, bool isHardDelete)
        {
           
                var report = await _reportReadRepository.GetByIdAsync(reportId);
                if (report == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Report not found",
                        statusCode: 404
                    );
                }

                var result = await _reportWriteRepository.DeleteAsync(report, isHardDelete);
                if (!result)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Failed to delete report",
                        statusCode: 400
                    );
                }

                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: $"Report {(isHardDelete ? "permanently deleted" : "archived")} successfully"
                );
           
        }
    }
}