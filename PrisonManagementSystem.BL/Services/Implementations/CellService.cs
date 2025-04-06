using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Cell;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.BL.Extensions;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class CellService : ICellService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICellReadRepository _cellReadRepository;
        private readonly ICellWriteRepository _cellWriteRepository;
        private readonly IPrisonerReadRepository _prisonerReadRepository;
        private readonly IPrisonReadRepository _prisonReadRepository;

        public CellService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cellReadRepository = _unitOfWork.GetRepository<ICellReadRepository>();
            _cellWriteRepository = _unitOfWork.GetRepository<ICellWriteRepository>();
            _prisonerReadRepository = _unitOfWork.GetRepository<IPrisonerReadRepository>();
            _prisonReadRepository = _unitOfWork.GetRepository<IPrisonReadRepository>();
        }

        // Helper method: Check if the prison has enough overall capacity for a new or updated cell
        private async Task<bool> IsPrisonCapacitySufficientAsync(Guid prisonId, int newCellCapacity)
        {
            var prison = await _prisonReadRepository.GetByIdAsync(prisonId);
            if (prison == null) return false;

            var cells = await _cellReadRepository.GetAllAsync(c => c.PrisonId == prisonId);
            if (cells == null) return false;

            var totalCapacity = cells.AsQueryable().Sum(c => c.Capacity);
            return totalCapacity + newCellCapacity <= prison.Capacity;
        }

        // Get paginated list of all cells
        public async Task<GenericResponseModel<PaginationResponse<GetCellDto>>> GetAllCellsAsync(PaginationRequest paginationRequest)
        {
            var cellsList = await _cellReadRepository.GetAllByPagingAsync(
                predicate: null,
                include: q => q
                    .Include(c => c.Prison) // Include related prison data
                    .Include(c => c.Prisoners), // Include related prisoners
                currentPage: paginationRequest.PageNumber,
                pageSize: paginationRequest.PageSize
            );

            if (cellsList == null || !cellsList.Any())
            {
                return GenericResponseModel<PaginationResponse<GetCellDto>>.SuccessResponse(
                    data: null,
                    statusCode: 200,
                    message: "No cells found"
                );
            }

            int totalCount = await _cellReadRepository.GetCountAsync();

            var cellDtos = cellsList.Select(cell => cell.ToGetCellDto()).ToList();
            var paginatedResponse = new PaginationResponse<GetCellDto>(totalCount, cellDtos, paginationRequest.PageNumber, paginationRequest.PageSize);

            return GenericResponseModel<PaginationResponse<GetCellDto>>.SuccessResponse(
                data: paginatedResponse,
                statusCode: 200,
                message: "Cells retrieved successfully"
            );
        }

        // Update a cell's information
        public async Task<GenericResponseModel<bool>> UpdateCellAsync(Guid cellId, UpdateCellDto updateCellDto)
        {
            var cell = await _cellReadRepository.GetByIdAsync(cellId);
            if (cell == null)
            {
                return GenericResponseModel<bool>.FailureResponse("Cell not found", 404);
            }

            // Ensure the updated capacity will not exceed the prison's total capacity
            bool isCapacitySufficient = await IsPrisonCapacitySufficientAsync(cell.PrisonId, updateCellDto.Capacity);
            if (!isCapacitySufficient)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "New cell capacity exceeds prison's total capacity", 400);
            }

            _mapper.Map(updateCellDto, cell);
            await _cellWriteRepository.UpdateAsync(cell);
            await _unitOfWork.CommitAsync();

            return GenericResponseModel<bool>.SuccessResponse(true, 200, "Cell updated successfully");
        }

        // Get a single cell by its ID
        public async Task<GenericResponseModel<GetCellDto>> GetCellByIdAsync(Guid cellId)
        {
            var cell = await _cellReadRepository.GetSingleAsync(
                predicate: c => c.Id == cellId,
                include: q => q
                    .Include(c => c.Prison)
                    .Include(c => c.Prisoners)
            );

            if (cell == null)
            {
                return GenericResponseModel<GetCellDto>.FailureResponse("Cell not found", 404);
            }

            var cellDto = cell.ToGetCellDto();

            return GenericResponseModel<GetCellDto>.SuccessResponse(cellDto, 200, "Cell retrieved successfully");
        }

        // Get all cells in a specific prison
        public async Task<GenericResponseModel<IEnumerable<GetCellDto>>> GetAllCellsByPrisonIdAsync(Guid prisonId)
        {
            var cells = await _cellReadRepository.GetAllAsync(
                predicate: c => c.PrisonId == prisonId,
                include: q => q
                    .Include(c => c.Prison)
                    .Include(c => c.Prisoners)
            );

            if (cells == null || !cells.Any())
            {
                return GenericResponseModel<IEnumerable<GetCellDto>>.SuccessResponse(
                    Enumerable.Empty<GetCellDto>(), 404, "No cells found for this prison");
            }

            var cellDtos = cells.Select(cell => cell.ToGetCellDto()).ToList();

            return GenericResponseModel<IEnumerable<GetCellDto>>.SuccessResponse(cellDtos, 200, "Cells retrieved successfully");
        }

        // Delete a cell (soft or hard delete)
        public async Task<GenericResponseModel<bool>> DeleteCellAsync(Guid cellId, bool isHardDelete)
        {
            var cell = await _cellReadRepository.GetSingleAsync(
                c => c.Id == cellId,
                include: q => q.Include(c => c.Prisoners)
            );

            if (cell == null)
            {
                return GenericResponseModel<bool>.FailureResponse("Cell not found", 404);
            }

            if (isHardDelete)
            {
                // Prevent hard deletion if there are active prisoners
                bool hasActivePrisoners = cell.Prisoners.Any(p => p.Status != PrisonerStatus.Released && p.Status != PrisonerStatus.Deceased);
                if (hasActivePrisoners)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "This cell still has active prisoners. You cannot hard delete it until they are removed.", 400);
                }
            }

            bool isDeleted = await _cellWriteRepository.DeleteAsync(cell, isHardDelete);

            if (!isDeleted)
            {
                return GenericResponseModel<bool>.FailureResponse("Error occurred while deleting the cell", 500);
            }

            await _unitOfWork.CommitAsync();

            return GenericResponseModel<bool>.SuccessResponse(
                true, 200, $"Cell successfully {(isHardDelete ? "hard deleted" : "soft deleted")}");
        }

        // Add a new cell
        public async Task<GenericResponseModel<CreateCellDto>> AddCellAsync(CreateCellDto createCellDto)
        {
            var isCapacitySufficient = await IsPrisonCapacitySufficientAsync(createCellDto.PrisonId, createCellDto.Capacity);

            if (!isCapacitySufficient)
            {
                return GenericResponseModel<CreateCellDto>.FailureResponse("Prison capacity exceeded or prison not found", 400);
            }

            // Prevent duplicate cell numbers in the same prison
            var existingCell = await _cellReadRepository.AnyAsync(
                c => c.CellNumber == createCellDto.CellNumber && c.PrisonId == createCellDto.PrisonId
            );

            if (existingCell)
            {
                return GenericResponseModel<CreateCellDto>.FailureResponse("Cell with this number already exists", 400);
            }

            var cell = _mapper.Map<Cell>(createCellDto);
            await _cellWriteRepository.AddAsync(cell);
            await _unitOfWork.CommitAsync();

            return GenericResponseModel<CreateCellDto>.SuccessResponse(
                _mapper.Map<CreateCellDto>(cell), 201, "Cell created successfully");
        }
    }


}
