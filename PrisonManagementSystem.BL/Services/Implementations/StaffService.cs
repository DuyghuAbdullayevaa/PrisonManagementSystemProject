using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.BL.DTOs.Staff;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonStaffRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IScheduleRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IStaffRepository;
using PrisonManagementSystem.DTOs;
using PrisonManagementSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using Serilog;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStaffReadRepository _staffReadRepository;
        private readonly IStaffWriteRepository _staffWriteRepository;
        private readonly IScheduleWriteRepository _scheduleWriteRepository;
        private readonly IScheduleReadRepository _scheduleReadRepository;
        private readonly IPrisonStaffWriteRepository _prisonStaffWriteRepository;
        private readonly IPrisonStaffReadRepository _prisonStaffReadRepository;

        public StaffService(
            IUnitOfWork unitOfWork,
            IMapper mapper
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            _staffReadRepository = _unitOfWork.GetRepository<IStaffReadRepository>();
            _staffWriteRepository = _unitOfWork.GetRepository<IStaffWriteRepository>();
            _scheduleWriteRepository = _unitOfWork.GetRepository<IScheduleWriteRepository>();
            _scheduleReadRepository = _unitOfWork.GetRepository<IScheduleReadRepository>();
            _prisonStaffWriteRepository = _unitOfWork.GetRepository<IPrisonStaffWriteRepository>();
            _prisonStaffReadRepository = _unitOfWork.GetRepository<IPrisonStaffReadRepository>();
        }

        public async Task<GenericResponseModel<PaginationResponse<GetStaffDto>>> GetAllStaffAsync(PaginationRequest paginationRequest)
        {
            // Log the start of the operation
                Log.Information($"Fetching staff members - Page: {paginationRequest.PageNumber}, Size: {paginationRequest.PageSize}");

                var staffEntities = await _staffReadRepository.GetAllByPagingAsync(
                    orderBy: q => q.OrderBy(s => s.Name),
                    include: s => s
                        .Include(x => x.Schedules)
                        .Include(p => p.PrisonStaffs)
                            .ThenInclude(ps => ps.Prison),
                    currentPage: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize
                );

                if (!staffEntities.Any())
                {
                    Log.Warning("No staff members found in database");
                    return GenericResponseModel<PaginationResponse<GetStaffDto>>.SuccessResponse(
                        data: new PaginationResponse<GetStaffDto>(0, new List<GetStaffDto>(), paginationRequest.PageNumber, paginationRequest.PageSize),
                        statusCode: 200,
                        message: "No staff members found"
                    );
                }

                int totalCount = await _staffReadRepository.GetCountAsync();
                var staffDtos = _mapper.Map<List<GetStaffDto>>(staffEntities);

                // Convert shift times to readable format for each schedule
                foreach (var staffDto in staffDtos)
                {
                    foreach (var schedule in staffDto.Schedules)
                    {
                        var shiftTimes = ShiftHelper.GetShiftTimes(schedule.ShiftType);
                        schedule.StartTime = shiftTimes.Start.ToString(@"hh\:mm");
                        schedule.EndTime = shiftTimes.End.ToString(@"hh\:mm");
                    }
                }

                var paginatedResponse = new PaginationResponse<GetStaffDto>(
                    totalCount,
                    staffDtos,
                    paginationRequest.PageNumber,
                    paginationRequest.PageSize
                );

                Log.Information($"Successfully retrieved {staffDtos.Count} staff members");
                return GenericResponseModel<PaginationResponse<GetStaffDto>>.SuccessResponse(
                    data: paginatedResponse,
                    statusCode: 200,
                    message: "Staff members retrieved successfully"
                );
          
        }
        public async Task<GenericResponseModel<bool>> CreateStaffAsync(CreateStaffDto createStaffDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {


                // Validate allowed positions for staff creation
                var allowedPositions = new List<PositionType>
                {
                    PositionType.Janitor,
                    PositionType.Cook,
                    PositionType.Psychologist,
                    PositionType.Nurse,
                    PositionType.Doctor
                };

                if (!allowedPositions.Contains(createStaffDto.Position))
                {
                    Log.Warning($"Invalid position attempted: {createStaffDto.Position}");
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Staff can only be created for specific positions",
                        statusCode: 400
                    );
                }

                // Check for duplicate email
                bool emailExists = await _staffReadRepository.AnyAsync(s => s.Email == createStaffDto.Email);
                if (emailExists)
                {
                    Log.Warning($"Duplicate email detected: {createStaffDto.Email}");
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "An employee with this email already exists",
                        statusCode: 400
                    );
                }

                // Validate schedule date matches start date
                if (createStaffDto.Schedules.Any(s => s.Date.Date != createStaffDto.DateOfStarting.Date))
                {
                    Log.Warning("Schedule date doesn't match start date");
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Schedule date must match the start date",
                        statusCode: 400
                    );
                }

                var staffEntity = _mapper.Map<Staff>(createStaffDto);
                staffEntity.UserId = null; // Staff without user account

                await _staffWriteRepository.AddAsync(staffEntity);

                // Create prison-staff relationship
                var prisonStaff = new PrisonStaff
                {
                    Id = Guid.NewGuid(),
                    StaffId = staffEntity.Id,
                    PrisonId = createStaffDto.PrisonId
                };
                await _prisonStaffWriteRepository.AddAsync(prisonStaff);

                // Create schedules for the staff member
                foreach (var schedule in createStaffDto.Schedules)
                {
                    var shiftTimes = ShiftHelper.GetShiftTimes(schedule.ShiftType);

                    var todaySchedule = new Schedule
                    {
                        Id = Guid.NewGuid(),
                        StaffId = staffEntity.Id,
                        Date = schedule.Date.Date,
                        ShiftType = schedule.ShiftType,
                        StartTime = schedule.Date.Date.Add(shiftTimes.Start),
                        EndTime = schedule.Date.Date.Add(shiftTimes.End)
                    };

                    await _scheduleWriteRepository.AddAsync(todaySchedule);
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 201,
                    message: "Staff member created successfully"
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Log.Error(ex, "Error creating staff member");
                return GenericResponseModel<bool>.FailureResponse(
                    error: $"An error occurred while creating staff: {ex.Message}",
                    statusCode: 500
                );
            }
        }

        public async Task<GenericResponseModel<bool>> UpdateStaffAsync(Guid id, UpdateStaffDto updateStaffDto)
        {
            await _unitOfWork.BeginTransactionAsync();

           
                

                var staffEntity = await _staffReadRepository.GetByIdAsync(id);
                if (staffEntity == null)
                {
                    Log.Warning($"Staff member with ID {id} not found");
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Staff member not found",
                        statusCode: 404
                    );
                }

                // Validate email uniqueness if changed
                if (staffEntity.Email != updateStaffDto.Email)
                {
                    bool emailExists = await _staffReadRepository.AnyAsync(s => s.Email == updateStaffDto.Email);
                    if (emailExists)
                    {
                        Log.Warning($"Email {updateStaffDto.Email} already exists in system");
                        return GenericResponseModel<bool>.FailureResponse(
                            "This email already belongs to another staff member",
                            400
                        );
                    }
                }

                _mapper.Map(updateStaffDto, staffEntity);

                // Update prison assignment if changed
                var prisonStaffEntity = await _prisonStaffReadRepository.GetSingleAsync(
                    ps => ps.StaffId == staffEntity.Id
                );

                if (prisonStaffEntity != null && prisonStaffEntity.PrisonId != updateStaffDto.PrisonId)
                {
                
                    prisonStaffEntity.PrisonId = updateStaffDto.PrisonId;
                    await _prisonStaffWriteRepository.UpdateAsync(prisonStaffEntity);
                }

                await _staffWriteRepository.UpdateAsync(staffEntity);
                await _unitOfWork.CommitAsync();

             
                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: "Staff member updated successfully"
                );
            
          
        }

        public async Task<GenericResponseModel<GetStaffDto>> GetStaffByIdAsync(Guid id)
        {
           
                Log.Information($"Fetching staff member with ID: {id}");

                var staffEntity = await _staffReadRepository.GetSingleAsync(
                    s => s.Id == id,
                    include: query => query
                        .Include(p => p.PrisonStaffs)
                            .ThenInclude(ps => ps.Prison)
                        .Include(s => s.Schedules)
                );

                if (staffEntity == null)
                {
                    Log.Warning($"Staff member with ID {id} not found");
                    return GenericResponseModel<GetStaffDto>.FailureResponse(
                        error: "Staff member not found",
                        statusCode: 404
                    );
                }

                var staffDto = _mapper.Map<GetStaffDto>(staffEntity);

                // Format shift times for display
                foreach (var schedule in staffDto.Schedules)
                {
                    var shiftTimes = ShiftHelper.GetShiftTimes(schedule.ShiftType);
                    schedule.StartTime = shiftTimes.Start.ToString(@"hh\:mm");
                    schedule.EndTime = shiftTimes.End.ToString(@"hh\:mm");
                }

                Log.Information($"Successfully retrieved staff member with ID {id}");
                return GenericResponseModel<GetStaffDto>.SuccessResponse(
                    data: staffDto,
                    statusCode: 200,
                    message: "Staff member retrieved successfully"
                );
          
        }


        public async Task<GenericResponseModel<bool>> DeleteStaffAsync(Guid id, bool isHardDelete)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                Log.Information($"Starting delete process for staff ID: {id} (Hard delete: {isHardDelete})");

                var staffEntity = await _staffReadRepository.GetByIdAsync(id);
                if (staffEntity == null)
                {
                    Log.Warning($"Staff member with ID {id} not found for deletion");
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Staff member not found",
                        statusCode: 404
                    );
                }

                // Soft delete all related schedules first
                var schedules = await _scheduleReadRepository.GetAllAsync(s => s.StaffId == id);
                foreach (var schedule in schedules)
                {
                    await _scheduleWriteRepository.SoftDeleteAsync(schedule);
                }

                var result = await _staffWriteRepository.DeleteAsync(staffEntity, isHardDelete);
                if (!result)
                {
                    Log.Error($"Failed to delete staff member with ID {id}");
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Error occurred while deleting staff member",
                        statusCode: 400
                    );
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                Log.Information($"Successfully deleted staff member with ID {id}");
                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: $"Staff member {(isHardDelete ? "permanently deleted" : "deactivated")} successfully"
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Log.Error(ex, $"Error deleting staff member with ID {id}");
                return GenericResponseModel<bool>.FailureResponse(
                    error: $"An error occurred while deleting staff: {ex.Message}",
                    statusCode: 500
                );
            }
        }
    }
}