using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.DTOs.Visitor;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitorRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitRepository;
using PrisonManagementSystem.DTOs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class VisitorService : IVisitorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IVisitorReadRepository _visitorReadRepository;
        private readonly IVisitorWriteRepository _visitorWriteRepository;
        private readonly IVisitWriteRepository _visitWriteRepository;
        private readonly UserManager<User> _userManager; // Ensure UserManager is injected correctly.

        public VisitorService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _visitorReadRepository = _unitOfWork.GetRepository<IVisitorReadRepository>();
            _visitorWriteRepository = _unitOfWork.GetRepository<IVisitorWriteRepository>();
            _visitWriteRepository = _unitOfWork.GetRepository<IVisitWriteRepository>();
            _userManager = userManager;  // Correctly initialize the UserManager
        }

        // Fetch visitor by ID
        public async Task<GenericResponseModel<GetVisitorDto>> GetVisitorByIdAsync(Guid id)
        {
           
                var visitor = await _visitorReadRepository.GetSingleAsync(
                    v => v.Id == id,
                    include: query => query.Include(v => v.VisitHistory)
                        .ThenInclude(vh => vh.Prisoner)
                );

                if (visitor == null)
                {
                    return GenericResponseModel<GetVisitorDto>.FailureResponse("Visitor not found", 404);
                }

                var visitorDto = _mapper.Map<GetVisitorDto>(visitor);
                return GenericResponseModel<GetVisitorDto>.SuccessResponse(visitorDto, 200, "Visitor retrieved successfully");
          
        }

        // Fetch all visitors with pagination
        public async Task<GenericResponseModel<PaginationResponse<GetVisitorDto>>> GetAllVisitorsAsync(PaginationRequest paginationRequest)
        {

            var visitors = await _visitorReadRepository.GetAllByPagingAsync(
                    include: q => q.Include(v => v.VisitHistory)
                                  .ThenInclude(vh => vh.Prisoner),
                    currentPage: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize
                );

            if (visitors == null || !visitors.Any())
            {
                return GenericResponseModel<PaginationResponse<GetVisitorDto>>.FailureResponse("No visitors found", 404);
            }

            int totalCount = await _visitorReadRepository.GetCountAsync();
            var visitorDtos = _mapper.Map<List<GetVisitorDto>>(visitors);
            var paginatedResponse = new PaginationResponse<GetVisitorDto>(totalCount, visitorDtos, paginationRequest.PageNumber, paginationRequest.PageSize);

            return GenericResponseModel<PaginationResponse<GetVisitorDto>>.SuccessResponse(paginatedResponse, 200, "Visitors retrieved successfully");
        }

        // Create visitor
        public async Task<GenericResponseModel<bool>> CreateVisitorAsync(CreateVisitorDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Check if the visitor is a professional (and thus should not be created in this method)
                if (dto.RelationToPrisoner == Relationship.Lawyer ||
                    dto.RelationToPrisoner == Relationship.Official ||
                    dto.RelationToPrisoner == Relationship.SocialWorker)
                {
                    return GenericResponseModel<bool>.FailureResponse("Professional visitors cannot be created in this method", 400);
                }

                // Check for existing visitor
                var existingVisitor = await _visitorReadRepository.AnyAsync(v =>
                    v.PhoneNumber == dto.PhoneNumber && v.Name == dto.Name);

                if (existingVisitor)
                {
                    return GenericResponseModel<bool>.FailureResponse("Visitor already exists", 400);
                }

                // Create visitor entity
                var visitor = _mapper.Map<Visitor>(dto);

                // Ensure UserId is null for non-professional visitors
                visitor.UserId = null;

                await _visitorWriteRepository.AddAsync(visitor);

                // Handle visit creation if provided
                if (dto.Visits != null)
                {
                    var visit = new Visit
                    {
                        Id = Guid.NewGuid(),
                        VisitorId = visitor.Id,
                        DurationInMinutes = dto.Visits.DurationInMinutes,
                        VisitDate = dto.Visits.VisitDate,
                        VisitType = dto.Visits.VisitType,
                        PrisonerId = dto.Visits.PrisonerId
                    };

                    await _visitWriteRepository.AddAsync(visit);
                }
                await _unitOfWork.CommitAsync();

                await _unitOfWork.CommitTransactionAsync();
                return GenericResponseModel<bool>.SuccessResponse(true, 201, "Visitor created successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Log.Error(ex, "Error creating visitor");
                return GenericResponseModel<bool>.FailureResponse($"Error creating visitor: {ex.Message}", 500);
            }
        }

        // Update visitor
        public async Task<GenericResponseModel<bool>> UpdateVisitorAsync(Guid id, UpdateVisitorDto updateVisitorDto)
        {
            var visitor = await _visitorReadRepository.GetByIdAsync(id);
            if (visitor == null)
            {
                return GenericResponseModel<bool>.FailureResponse("Visitor not found", 404);
            }

            // Update visitor's basic details
            _mapper.Map(updateVisitorDto, visitor);

            // Update the relationship type
            visitor.RelationToPrisoner = updateVisitorDto.Relationship;

            // Handle the UserId based on relationship
            if (updateVisitorDto.UserId != null &&
                (updateVisitorDto.Relationship == Relationship.Lawyer ||
                 updateVisitorDto.Relationship == Relationship.Official ||
                 updateVisitorDto.Relationship == Relationship.SocialWorker
               ))
            {
                // Verify if the user exists
                var user = await _userManager.FindByIdAsync(updateVisitorDto.UserId);
                if (user == null)
                {
                    return GenericResponseModel<bool>.FailureResponse("Associated user not found", 404);
                }

                visitor.UserId = updateVisitorDto.UserId;
            }
            else
            {
                // Set UserId to null for non-professional visitors
                visitor.UserId = null;
            }

            // Update the visitor entity
            var result = await _visitorWriteRepository.UpdateAsync(visitor);
            if (!result)
            {
                return GenericResponseModel<bool>.FailureResponse("Failed to update visitor", 400);
            }

            // Commit changes
            await _unitOfWork.CommitAsync();

            return GenericResponseModel<bool>.SuccessResponse(true, 200, "Visitor updated successfully");
        }

        // Delete visitor
        public async Task<GenericResponseModel<bool>> DeleteVisitorAsync(Guid id, bool isHardDelete)
        {
            
                var visitor = await _visitorReadRepository.GetSingleAsync(
                    v => v.Id == id,
                    include: v => v.Include(vis => vis.VisitHistory)
                );

                if (visitor == null)
                {
                    return GenericResponseModel<bool>.FailureResponse("Visitor not found", 404);
                }

                var result = await _visitorWriteRepository.DeleteAsync(visitor, isHardDelete);
                if (!result)
                {
                    return GenericResponseModel<bool>.FailureResponse("Failed to delete visitor", 400);
                }

                await _unitOfWork.CommitAsync();
                return GenericResponseModel<bool>.SuccessResponse(true, 200, "Visitor deleted successfully");
           
        }
    }
}
