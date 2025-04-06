using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Extensions;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICrimeRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerCrimeRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class PrisonerService : IPrisonerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPrisonerReadRepository _prisonerReadRepository;
        private readonly IPrisonerWriteRepository _prisonerWriteRepository;
        private readonly ICellReadRepository _cellReadRepository;
        private readonly ICellWriteRepository _cellWriteRepository;
        private readonly IPrisonReadRepository _prisonReadRepository;
        private readonly IPrisonWriteRepository _prisonWriteRepository;
        private readonly IPrisonerCrimeWriteRepository _prisonerCrimeWriteRepository;
        private readonly ICrimeWriteRepository _crimeWriteRepository;

        public PrisonerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _prisonerReadRepository = _unitOfWork.GetRepository<IPrisonerReadRepository>();
            _prisonerWriteRepository = _unitOfWork.GetRepository<IPrisonerWriteRepository>();
            _cellReadRepository = _unitOfWork.GetRepository<ICellReadRepository>();
            _cellWriteRepository = _unitOfWork.GetRepository<ICellWriteRepository>();
            _prisonReadRepository = _unitOfWork.GetRepository<IPrisonReadRepository>();
            _prisonWriteRepository = _unitOfWork.GetRepository<IPrisonWriteRepository>();
            _crimeWriteRepository = _unitOfWork.GetRepository<ICrimeWriteRepository>();
            _prisonerCrimeWriteRepository = _unitOfWork.GetRepository<IPrisonerCrimeWriteRepository>();
        }

        public async Task<GenericResponseModel<PaginationResponse<GetPrisonerDto>>> GetAllPrisonersAsync(PaginationRequest request)
        {
          
                var prisoners = await _prisonerReadRepository.GetAllByPagingAsync(
                    include: q => q
                        .Include(p => p.Cell)
                        .Include(p => p.PrisonerCrimes).ThenInclude(pc => pc.Crime)
                        .Include(p => p.PrisonerPunishments).ThenInclude(pp => pp.Punishment)
                        .Include(p => p.PrisonersIncidents).ThenInclude(pi => pi.Incident),
                    orderBy: q => q.OrderBy(p => p.FirstName),
                    currentPage: request.PageNumber,
                    pageSize: request.PageSize
                );

                int totalCount = await _prisonerReadRepository.GetCountAsync();
                var prisonerDtos = prisoners.Select(p => p.ToGetPrisonerDto()).ToList();

                return GenericResponseModel<PaginationResponse<GetPrisonerDto>>.SuccessResponse(
                    new PaginationResponse<GetPrisonerDto>(totalCount, prisonerDtos, request.PageNumber, request.PageSize),
                    (int)HttpStatusCode.OK,
                    prisoners.Any() ? "Prisoners retrieved successfully" : "No prisoners found"
                );
           
        }

        public async Task<GenericResponseModel<GetPrisonerDto>> GetPrisonerByIdAsync(Guid id)
        {
          

                var prisoner = await _prisonerReadRepository.GetSingleAsync(
                    p => p.Id == id,
                    include: q => q
                        .Include(p => p.PrisonerPunishments).ThenInclude(pp => pp.Punishment)
                        .Include(p => p.PrisonerCrimes).ThenInclude(pc => pc.Crime)
                        .Include(p => p.PrisonersIncidents).ThenInclude(pi => pi.Incident)
                        .Include(p => p.Visits)
                        .Include(p => p.Cell)
                );

                if (prisoner == null)
                {
                    return GenericResponseModel<GetPrisonerDto>.FailureResponse(
                        "Prisoner not found",
                        (int)HttpStatusCode.NotFound);
                }

                return GenericResponseModel<GetPrisonerDto>.SuccessResponse(
                    prisoner.ToGetPrisonerDto(),
                    (int)HttpStatusCode.OK,
                    "Prisoner retrieved successfully");
           
        }

        public async Task<GenericResponseModel<bool>> AddPrisonerAsync(CreatePrisonerDto dto)
        {
            
            var existingPrisoner = await _prisonerReadRepository
                .GetAllAsync(p => p.FirstName == dto.FirstName &&
                                 p.LastName == dto.LastName &&
                                 p.DateOfBirth == dto.DateOfBirth);

            if (existingPrisoner.Any())
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "A prisoner with the same name and date of birth already exists.",
                    (int)HttpStatusCode.BadRequest);
            }

            var cell = await _cellReadRepository.GetByIdAsync(dto.CellId);
            if (cell == null || cell.Status == CellStatus.UnderMaintenance || cell.CurrentOccupancy >= cell.Capacity)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Cell is full or under maintenance",
                    (int)HttpStatusCode.BadRequest);
            }

            // Only begin transaction after validation passes
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var prisoner = _mapper.Map<Prisoner>(dto);
                prisoner.Gender = dto.Gender;
                prisoner.Status = dto.ReleaseDate.HasValue ? dto.Status : PrisonerStatus.LifeSentence;

                var prison = await _prisonReadRepository.GetByIdAsync(cell.PrisonId, true);
                if (prison == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Prison not found",
                        (int)HttpStatusCode.NotFound);
                }

                cell.CurrentOccupancy++;
                if (cell.CurrentOccupancy >= cell.Capacity)
                    cell.Status = CellStatus.Full;
                prison.CurrentInmates++;

                await _cellWriteRepository.UpdateAsync(cell);
                await _prisonWriteRepository.UpdateAsync(prison);
                await _prisonerWriteRepository.AddAsync(prisoner);

                foreach (var crimeDto in dto.Crimes)
                {
                    var crime = new Crime
                    {
                        Details = crimeDto.Details,
                        SeverityLevel = crimeDto.SeverityLevel,
                        Type = crimeDto.Type
                    };

                    await _crimeWriteRepository.AddAsync(crime);
                    await _prisonerCrimeWriteRepository.AddAsync(new PrisonerCrime
                    {
                        PrisonerId = prisoner.Id,
                        CrimeId = crime.Id
                    });
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    true,
                    (int)HttpStatusCode.Created,
                    "Prisoner added successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding prisoner: {FirstName} {LastName}", dto.FirstName, dto.LastName);
                await _unitOfWork.RollbackTransactionAsync();
                return GenericResponseModel<bool>.FailureResponse(
                    $"Error adding prisoner: {ex.Message}",
                    (int)HttpStatusCode.InternalServerError);
            }
        }
        public async Task<GenericResponseModel<GetPrisonerDto>> UpdatePrisonerAsync(Guid id, UpdatePrisonerDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var prisoner = await _prisonerReadRepository.GetByIdAsync(id);
                if (prisoner == null)
                {
                    return GenericResponseModel<GetPrisonerDto>.FailureResponse(
                        "Prisoner not found",
                        (int)HttpStatusCode.NotFound);
                }

                if ((prisoner.Status == PrisonerStatus.LifeSentence && dto.Status != PrisonerStatus.LifeSentence) ||
                    prisoner.Status == PrisonerStatus.Deceased)
                {
                    return GenericResponseModel<GetPrisonerDto>.FailureResponse(
                        prisoner.Status == PrisonerStatus.LifeSentence
                            ? "Cannot change status of life-sentenced prisoner"
                            : "Cannot update deceased prisoner",
                        (int)HttpStatusCode.BadRequest);
                }

                if (prisoner.ReleaseDate.HasValue && prisoner.ReleaseDate.Value <= DateTime.UtcNow)
                {
                    if (dto.Status == PrisonerStatus.Released && prisoner.Status != PrisonerStatus.Released)
                    {
                        return GenericResponseModel<GetPrisonerDto>.FailureResponse(
                            "Prisoner should be released as sentence has ended",
                            (int)HttpStatusCode.BadRequest);
                    }
                }
                else if (dto.Status == PrisonerStatus.Released)
                {
                    return GenericResponseModel<GetPrisonerDto>.FailureResponse(
                        "Cannot release prisoner before sentence ends",
                        (int)HttpStatusCode.BadRequest);
                }

                if (prisoner.CellId != dto.CellId)
                {
                    var oldCell = await _cellReadRepository.GetByIdAsync(prisoner.CellId);
                    if (oldCell != null)
                    {
                        oldCell.CurrentOccupancy--;
                        if (oldCell.CurrentOccupancy == 0)
                            oldCell.Status = CellStatus.Available;

                        await _cellWriteRepository.UpdateAsync(oldCell);

                        var oldPrison = await _prisonReadRepository.GetByIdAsync(oldCell.PrisonId);
                        if (oldPrison != null)
                        {
                            oldPrison.CurrentInmates--;
                            await _prisonWriteRepository.UpdateAsync(oldPrison);
                        }
                    }

                    var newCell = await _cellReadRepository.GetByIdAsync(dto.CellId);
                    if (newCell == null || newCell.Status == CellStatus.UnderMaintenance || newCell.CurrentOccupancy >= newCell.Capacity)
                    {
                        return GenericResponseModel<GetPrisonerDto>.FailureResponse(
                            "New cell is unavailable",
                            (int)HttpStatusCode.BadRequest);
                    }

                    newCell.CurrentOccupancy++;
                    if (newCell.Status == CellStatus.Available)
                        newCell.Status = CellStatus.Occupied;

                    await _cellWriteRepository.UpdateAsync(newCell);

                    var newPrison = await _prisonReadRepository.GetByIdAsync(newCell.PrisonId);
                    if (newPrison != null)
                    {
                        newPrison.CurrentInmates++;
                        await _prisonWriteRepository.UpdateAsync(newPrison);
                    }

                    prisoner.CellId = dto.CellId;
                }

                _mapper.Map(dto, prisoner);
                prisoner.Status = dto.Status;

                await _prisonerWriteRepository.UpdateAsync(prisoner);
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return GenericResponseModel<GetPrisonerDto>.SuccessResponse(
                    _mapper.Map<GetPrisonerDto>(prisoner),
                    (int)HttpStatusCode.OK,
                    "Prisoner updated successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return GenericResponseModel<GetPrisonerDto>.FailureResponse(
                    $"Error updating prisoner: {ex.Message}",
                    (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResponseModel<bool>> DeletePrisonerAsync(Guid id, bool isHardDelete)
        {

                var prisoner = await _prisonerReadRepository.GetByIdAsync(id);
                if (prisoner == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Prisoner not found",
                        (int)HttpStatusCode.NotFound);
                }

                if (prisoner.Status == PrisonerStatus.Released || prisoner.Status == PrisonerStatus.Deceased)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Cannot delete released or deceased prisoner",
                        (int)HttpStatusCode.BadRequest);
                }

                var cell = await _cellReadRepository.GetByIdAsync(prisoner.CellId);
                if (cell != null)
                {
                    cell.CurrentOccupancy--;
                    if (cell.CurrentOccupancy < cell.Capacity)
                        cell.Status = CellStatus.Available;
                    await _cellWriteRepository.UpdateAsync(cell);
                }
                else
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Prisoner cell not found",
                        (int)HttpStatusCode.NotFound);
                }

                var prison = await _prisonReadRepository.GetSingleAsync(
                    p => p.Id == cell.PrisonId,
                    include: q => q.Include(p => p.Cells));

                if (prison != null && prison.CurrentInmates > 0)
                {
                    prison.CurrentInmates--;
                    await _prisonWriteRepository.UpdateAsync(prison);
                }
                else
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Prison not found or has no inmates",
                        (int)HttpStatusCode.NotFound);
                }

                var result = await _prisonerWriteRepository.DeleteAsync(prisoner, isHardDelete);
                if (!result)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Failed to delete prisoner",
                        (int)HttpStatusCode.BadRequest);
                }

                await _unitOfWork.CommitAsync();
                return GenericResponseModel<bool>.SuccessResponse(
                    true,
                    (int)HttpStatusCode.OK,
                    $"Prisoner {(isHardDelete ? "permanently deleted" : "archived")} successfully");
            
        }
    }
}