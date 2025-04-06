using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.BL.DTOs.Prison;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonStaffRepository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class PrisonService : IPrisonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPrisonReadRepository _prisonReadRepository;
        private readonly IPrisonWriteRepository _prisonWriteRepository;
        private readonly IPrisonerReadRepository _prisonerReadRepository;
        private readonly IPrisonStaffReadRepository _prisonStaffReadRepository;
        private readonly ICellReadRepository _cellReadRepository;
        private readonly ICellWriteRepository _cellWriteRepository;

        public PrisonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _prisonReadRepository = _unitOfWork.GetRepository<IPrisonReadRepository>();
            _prisonWriteRepository = _unitOfWork.GetRepository<IPrisonWriteRepository>();
            _prisonerReadRepository = _unitOfWork.GetRepository<IPrisonerReadRepository>();
            _prisonStaffReadRepository = _unitOfWork.GetRepository<IPrisonStaffReadRepository>();
            _cellReadRepository = _unitOfWork.GetRepository<ICellReadRepository>();
            _cellWriteRepository = _unitOfWork.GetRepository<ICellWriteRepository>();
        }

        public async Task<GenericResponseModel<PaginationResponse<GetPrisonDto>>> GetAllPrisonsAsync(PaginationRequest paginationRequest)
        {
            var prisons = await _prisonReadRepository.GetAllByPagingAsync(
                include: query => query
                    .Include(p => p.Cells)
                    .Include(p => p.PrisonStaffs)
                        .ThenInclude(ps => ps.Staff)
                    .Include(p => p.Incidents),
                currentPage: paginationRequest.PageNumber,
                pageSize: paginationRequest.PageSize
            );

            if (!prisons.Any())
            {
                Log.Warning("No prisons found");
                return GenericResponseModel<PaginationResponse<GetPrisonDto>>.SuccessResponse(
                    data: new PaginationResponse<GetPrisonDto>(0, new List<GetPrisonDto>(),
                        paginationRequest.PageNumber, paginationRequest.PageSize),
                    statusCode: 200,
                    message: "No prisons found"
                );
            }

            int totalCount = await _prisonReadRepository.GetCountAsync();
            var prisonDtos = _mapper.Map<List<GetPrisonDto>>(prisons);

            return GenericResponseModel<PaginationResponse<GetPrisonDto>>.SuccessResponse(
                data: new PaginationResponse<GetPrisonDto>(totalCount, prisonDtos,
                    paginationRequest.PageNumber, paginationRequest.PageSize),
                statusCode: 200,
                message: "Prisons retrieved successfully"
            );
        }

        public async Task<GenericResponseModel<GetPrisonDto>> GetPrisonByIdAsync(Guid id)
        {
            var prison = await _prisonReadRepository.GetSingleAsync(
                p => p.Id == id,
                include: query => query
                    .Include(p => p.Cells)
                    .Include(p => p.PrisonStaffs)
                        .ThenInclude(ps => ps.Staff)
                    .Include(p => p.Incidents)
            );

            if (prison == null)
            {
                return GenericResponseModel<GetPrisonDto>.FailureResponse(
                    error: "Prison not found",
                    statusCode: 404
                );
            }

            return GenericResponseModel<GetPrisonDto>.SuccessResponse(
                data: _mapper.Map<GetPrisonDto>(prison),
                statusCode: 200,
                message: "Prison retrieved successfully"
            );
        }

        public async Task<GenericResponseModel<bool>> CreatePrisonAsync(CreatePrisonDto prisonCreateDto)
        {
            var existingPrison = await _prisonReadRepository.GetSingleAsync(
                p => p.Name == prisonCreateDto.Name && p.Location == prisonCreateDto.Location);

            if (existingPrison != null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Prison with this name and location already exists",
                    statusCode: 400
                );
            }

            var prison = _mapper.Map<Prison>(prisonCreateDto);
            var result = await _prisonWriteRepository.AddAsync(prison);

            if (!result)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Failed to create prison",
                    statusCode: 400
                );
            }

            await _unitOfWork.CommitAsync();
            Log.Information($"New prison created: {prisonCreateDto.Name} ({prisonCreateDto.Location})");

            return GenericResponseModel<bool>.SuccessResponse(
                data: true,
                statusCode: 201,
                message: "Prison created successfully"
            );
        }


        public async Task<GenericResponseModel<bool>> UpdatePrisonAsync(Guid id, UpdatePrisonDto prisonUpdateDto)
        {
            var prison = await _prisonReadRepository.GetByIdAsync(id, true);
            if (prison == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Prison not found",
                    statusCode: 404
                );
            }

            if (prison.Status == PrisonStatus.Closed)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Cannot update a closed prison",
                    statusCode: 400
                );
            }

            if (prison.Status == PrisonStatus.Active && !prisonUpdateDto.IsActive)
            {
                var prisonerCount = await _prisonerReadRepository.GetCountAsync(p => p.Cell.PrisonId == id);
                var staffCount = await _prisonStaffReadRepository.GetCountAsync(s => s.PrisonId == id);

                if (prisonerCount > 0 || staffCount > 0)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Cannot close prison with active prisoners or staff",
                        statusCode: 400
                    );
                }
            }

            _mapper.Map(prisonUpdateDto, prison);
            var result = await _prisonWriteRepository.UpdateAsync(prison);

            if (!result)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Failed to update prison",
                    statusCode: 400
                );
            }

            await _unitOfWork.CommitAsync();
            return GenericResponseModel<bool>.SuccessResponse(
                data: true,
                statusCode: 200,
                message: "Prison updated successfully"
            );
        }

        public async Task<GenericResponseModel<bool>> DeletePrisonAsync(Guid id, bool isHardDelete)
        {
            var prison = await _prisonReadRepository.GetByIdAsync(id, true);
            if (prison == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Prison not found",
                    statusCode: 404
                );
            }

            var hasActiveStaff = await _prisonStaffReadRepository.AnyAsync(s => s.PrisonId == id);
            if (hasActiveStaff)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Cannot delete prison with active staff",
                    statusCode: 400
                );
            }

            var cells = await _cellReadRepository.GetAllAsync(c => c.PrisonId == id);
            foreach (var cell in cells)
            {
                var hasActivePrisoners = await _prisonerReadRepository.AnyAsync(
                    p => p.CellId == cell.Id && p.Status == PrisonerStatus.Active);

                if (hasActivePrisoners)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: $"Cannot delete prison with active prisoners in cell {cell.CellNumber}",
                        statusCode: 400
                    );
                }

                var resultCell = await _cellWriteRepository.SoftDeleteAsync(cell);
                if (!resultCell)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: $"Failed to delete cell {cell.CellNumber}",
                        statusCode: 400
                    );
                }
            }

            var result = await _prisonWriteRepository.DeleteAsync(prison, isHardDelete);
            if (!result)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Failed to delete prison",
                    statusCode: 400
                );
            }

            await _unitOfWork.CommitAsync();
            return GenericResponseModel<bool>.SuccessResponse(
                data: true,
                statusCode: 200,
                message: $"Prison {(isHardDelete ? "permanently deleted" : "archived")} successfully"
            );
        }

        public async Task<GenericResponseModel<bool>> ChangePrisonStatusAsync(Guid id, bool isActive)
        {
            var prison = await _prisonReadRepository.GetByIdAsync(id, true);
            if (prison == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    error: "Prison not found",
                    statusCode: 404
                );
            }

            if (!isActive) { 
                var hasActivePrisoners = await _prisonerReadRepository.AnyAsync(
                    p => p.Cell.PrisonId == id && p.Status == PrisonerStatus.Active);

                if (hasActivePrisoners)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Cannot change status of prison with active prisoners",
                        statusCode: 400
                    );
                }

                var hasActiveStaff = await _prisonStaffReadRepository.AnyAsync(s => s.PrisonId == id);
                if (hasActiveStaff)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Cannot close prison with active staff",
                        statusCode: 400
                    );
                }
            }

            prison.Status = isActive ? PrisonStatus.Active : PrisonStatus.Closed;
            await _prisonWriteRepository.UpdateAsync(prison);
            await _unitOfWork.CommitAsync();

            return GenericResponseModel<bool>.SuccessResponse(
                data: true,
                statusCode: 200,
                message: $"Prison status changed to {(isActive ? "Active" : "Closed")}"
            );
        }
    }
 }
