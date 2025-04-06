using AutoMapper;
using Microsoft.EntityFrameworkCore;

using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitorRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.BL.DTOs.ResponseModel;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class VisitService : IVisitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IVisitReadRepository _visitReadRepository;
        private readonly IVisitWriteRepository _visitWriteRepository;
        private readonly IPrisonerReadRepository _prisonerReadRepository;
        private readonly IVisitorReadRepository _visitorReadRepository;

        public VisitService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _visitReadRepository = _unitOfWork.GetRepository<IVisitReadRepository>();
            _visitWriteRepository = _unitOfWork.GetRepository<IVisitWriteRepository>();
            _prisonerReadRepository = _unitOfWork.GetRepository<IPrisonerReadRepository>();
            _visitorReadRepository = _unitOfWork.GetRepository<IVisitorReadRepository>();
        }

        public async Task<GenericResponseModel<GetVisitDto>> GetVisitByIdAsync(Guid id)
        {
           
                var visit = await _visitReadRepository.GetSingleAsync(
                    v => v.Id == id,
                    include: query => query
                        .Include(v => v.Prisoner)
                        .Include(v => v.Visitor)
                );

                if (visit == null)
                {
                    return GenericResponseModel<GetVisitDto>.FailureResponse(
                        error: "Visit not found",
                        statusCode: 404
                    );
                }

                return GenericResponseModel<GetVisitDto>.SuccessResponse(
                    data: _mapper.Map<GetVisitDto>(visit),
                    statusCode: 200,
                    message: "Visit retrieved successfully"
                );
          
        }

        public async Task<GenericResponseModel<PaginationResponse<GetVisitDto>>> GetAllVisitsAsync(PaginationRequest paginationRequest)
        {
            
                var visits = await _visitReadRepository.GetAllByPagingAsync(
                    include: query => query
                        .Include(v => v.Prisoner)
                        .Include(v => v.Visitor),
                    currentPage: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize
                );

                if (!visits.Any())
                {
                    return GenericResponseModel<PaginationResponse<GetVisitDto>>.SuccessResponse(
                        data: new PaginationResponse<GetVisitDto>(0, new List<GetVisitDto>(),
                               paginationRequest.PageNumber, paginationRequest.PageSize),
                        statusCode: 200,
                        message: "No visits found"
                    );
                }

                int totalCount = await _visitReadRepository.GetCountAsync();
                var visitDtos = _mapper.Map<List<GetVisitDto>>(visits);

                return GenericResponseModel<PaginationResponse<GetVisitDto>>.SuccessResponse(
                    data: new PaginationResponse<GetVisitDto>(totalCount, visitDtos,
                           paginationRequest.PageNumber, paginationRequest.PageSize),
                    statusCode: 200,
                    message: "Visits retrieved successfully"
                );
            
           
        }

        public async Task<GenericResponseModel<GetVisitDto>> CreateVisitAsync(CreateVisitDto createVisitDto)
        {
           
                bool visitExists = await _visitReadRepository
                    .AnyAsync(v => v.PrisonerId == createVisitDto.PrisonerId &&
                                   v.VisitDate == createVisitDto.VisitDate);

                if (visitExists)
                {
                    return GenericResponseModel<GetVisitDto>.FailureResponse(
                        error: "Prisoner already has a visit scheduled for this date",
                        statusCode: 400
                    );
                }

                var prisoner = await _prisonerReadRepository.GetByIdAsync(createVisitDto.PrisonerId);
                if (prisoner == null)
                {
                    return GenericResponseModel<GetVisitDto>.FailureResponse(
                        error: "Prisoner not found",
                        statusCode: 404
                    );
                }

                var visit = _mapper.Map<Visit>(createVisitDto);

                // Assuming that the visitType should come from the DTO
                visit.VisitType = createVisitDto.VisitType;

                await _visitWriteRepository.AddAsync(visit);
                await _unitOfWork.CommitAsync();

                return GenericResponseModel<GetVisitDto>.SuccessResponse(
                    data: _mapper.Map<GetVisitDto>(visit),
                    statusCode: 201,
                    message: "Visit created successfully"
                );
          
        }


        public async Task<GenericResponseModel<bool>> UpdateVisitAsync(Guid id, UpdateVisitDto updateVisitDto)
        {
           
            var existingVisit = await _visitReadRepository.GetByIdAsync(id);
                if (existingVisit == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Visit not found",
                        statusCode: 404
                    );
                }

                if (existingVisit.VisitDate < DateTime.UtcNow)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Cannot update past visits",
                        statusCode: 400
                    );
                }

                var visitorExists = await _visitorReadRepository
                    .AnyAsync(v => v.Id == existingVisit.VisitorId);
                if (!visitorExists)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Cannot update visit with deleted visitor",
                        statusCode: 400
                    );
                }

                // Update visitType if it has changed
                if (existingVisit.VisitType != updateVisitDto.VisitType)
                {
                    existingVisit.VisitType = updateVisitDto.VisitType;
                }

                // Map other properties from the DTO to the existing visit
                _mapper.Map(updateVisitDto, existingVisit);

                await _visitWriteRepository.UpdateAsync(existingVisit);
                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: "Visit updated successfully"
                );
           
        }

        public async Task<GenericResponseModel<bool>> DeleteVisitAsync(Guid id, bool isHardDelete)
        {
          
                var existingVisit = await _visitReadRepository.GetByIdAsync(id);
                if (existingVisit == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Visit not found",
                        statusCode: 404
                    );
                }

                if (existingVisit.VisitDate < DateTime.UtcNow)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Cannot delete past visits",
                        statusCode: 400
                    );
                }

                var result = await _visitWriteRepository.DeleteAsync(existingVisit, isHardDelete);
                if (!result)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Failed to delete visit",
                        statusCode: 400
                    );
                }

                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: $"Visit {(isHardDelete ? "permanently deleted" : "cancelled")} successfully"
                );
           
        }
    }
}
