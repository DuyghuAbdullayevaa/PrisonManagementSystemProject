using AutoMapper;
using Microsoft.EntityFrameworkCore;

using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IScheduleRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IStaffRepository;
using PrisonManagementSystem.DTOs;
using PrisonManagementSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrisonManagementSystem.BL.DTOs.ResponseModel;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IScheduleReadRepository _scheduleReadRepository;
        private readonly IScheduleWriteRepository _scheduleWriteRepository;
        private readonly IStaffReadRepository _staffReadRepository;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _scheduleReadRepository = _unitOfWork.GetRepository<IScheduleReadRepository>();
            _scheduleWriteRepository = _unitOfWork.GetRepository<IScheduleWriteRepository>();
            _staffReadRepository = _unitOfWork.GetRepository<IStaffReadRepository>();
        }

        public async Task<GenericResponseModel<bool>> AssignScheduleToStaffAsync(CreateScheduleDto createScheduleDto)
        {
            
                foreach (var staffId in createScheduleDto.StaffIds)
                {
                    var staff = await _staffReadRepository.GetByIdAsync(staffId);
                    if (staff == null)
                    {
                        return GenericResponseModel<bool>.FailureResponse(
                            error: $"Staff with ID {staffId} not found",
                            statusCode: 404
                        );
                    }

                    bool isScheduleExists = await _scheduleReadRepository
                        .AnyAsync(s => s.StaffId == staffId && s.Date == createScheduleDto.Date);

                    if (isScheduleExists)
                    {
                        return GenericResponseModel<bool>.FailureResponse(
                            error: "Staff member already has a shift assigned for this date",
                            statusCode: 400
                        );
                    }

                    var newSchedule = _mapper.Map<Schedule>(createScheduleDto);
                    newSchedule.StaffId = staffId;
                    newSchedule.ShiftType = createScheduleDto.ShiftType; // Use ShiftType from DTO

                    var shiftTimes = ShiftHelper.GetShiftTimes(createScheduleDto.ShiftType); // Use ShiftType from DTO
                    newSchedule.StartTime = DateTime.Today.Add(shiftTimes.Start);
                    newSchedule.EndTime = DateTime.Today.Add(shiftTimes.End);

                    await _scheduleWriteRepository.AddAsync(newSchedule);
                }

                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 201,
                    message: "Shifts assigned successfully"
                );
            
           
        }


        public async Task<GenericResponseModel<PaginationResponse<GetScheduleDto>>> GetAllSchedulesAsync(PaginationRequest paginationRequest)
        {
            
                var schedules = await _scheduleReadRepository.GetAllByPagingAsync(
                    include: query => query.Include(s => s.Staff),
                    currentPage: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize
                );

                if (!schedules.Any())
                {
                    return GenericResponseModel<PaginationResponse<GetScheduleDto>>.SuccessResponse(
                        data: new PaginationResponse<GetScheduleDto>(0, new List<GetScheduleDto>(),
                               paginationRequest.PageNumber, paginationRequest.PageSize),
                        statusCode: 200,
                        message: "No schedules found"
                    );
                }

                int totalCount = await _scheduleReadRepository.GetCountAsync();
                var scheduleDtos = _mapper.Map<List<GetScheduleDto>>(schedules);

                foreach (var scheduleDto in scheduleDtos)
                {
                    var shiftTimes = ShiftHelper.GetShiftTimes(scheduleDto.ShiftType);
                    scheduleDto.StartTime = shiftTimes.Start.ToString(@"hh\:mm");
                    scheduleDto.EndTime = shiftTimes.End.ToString(@"hh\:mm");
                }

                return GenericResponseModel<PaginationResponse<GetScheduleDto>>.SuccessResponse(
                    data: new PaginationResponse<GetScheduleDto>(totalCount, scheduleDtos,
                           paginationRequest.PageNumber, paginationRequest.PageSize),
                    statusCode: 200,
                    message: "Schedules retrieved successfully"
                );
           
        }

        public async Task<GenericResponseModel<GetScheduleDto>> GetScheduleByIdAsync(Guid id)
        {
           
            var schedule = await _scheduleReadRepository.GetSingleAsync(
                    s => s.Id == id,
                    include: query => query.Include(s => s.Staff)
                );

                if (schedule == null)
                {
                    return GenericResponseModel<GetScheduleDto>.FailureResponse(
                        error: "Schedule not found",
                        statusCode: 404
                    );
                }

                var scheduleDto = _mapper.Map<GetScheduleDto>(schedule);
                var shiftTimes = ShiftHelper.GetShiftTimes(scheduleDto.ShiftType);

                scheduleDto.StartTime = shiftTimes.Start.ToString(@"hh\:mm");
                scheduleDto.EndTime = shiftTimes.End.ToString(@"hh\:mm");

                return GenericResponseModel<GetScheduleDto>.SuccessResponse(
                    data: scheduleDto,
                    statusCode: 200,
                    message: "Schedule retrieved successfully"
                );
           
        }

        public async Task<GenericResponseModel<bool>> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleDto updateScheduleDto)
        {
           
            var schedule = await _scheduleReadRepository.GetByIdAsync(scheduleId);
                if (schedule == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Schedule not found",
                        statusCode: 404
                    );
                }

                // Map the update DTO to the schedule entity
                _mapper.Map(updateScheduleDto, schedule);

                // Update the ShiftType from the DTO
                schedule.ShiftType = updateScheduleDto.ShiftType;

                await _scheduleWriteRepository.UpdateAsync(schedule);
                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: "Schedule updated successfully"
                );
          
        }


        public async Task<GenericResponseModel<bool>> DeleteScheduleAsync(Guid id, bool isHardDelete)
        {
            

                var schedule = await _scheduleReadRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Schedule not found",
                        statusCode: 404
                    );
                }

                var result = await _scheduleWriteRepository.DeleteAsync(schedule, isHardDelete);
                if (!result)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Failed to delete schedule",
                        statusCode: 400
                    );
                }

                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: $"Schedule {(isHardDelete ? "permanently deleted" : "archived")} successfully"
                );
           
        }
    }
}