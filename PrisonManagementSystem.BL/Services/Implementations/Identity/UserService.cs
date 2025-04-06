using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.BL.DTOs.Identiity.User;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Services.Abstractions.Identity;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonStaffRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IScheduleRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IStaffRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitorRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitRepository;
using PrisonManagementSystem.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Implementations.Identity
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStaffWriteRepository _staffWriteRepository;
        private readonly IPrisonStaffWriteRepository _prisonStaffWriteRepository;
        private readonly IScheduleWriteRepository _scheduleWriteRepository;
        private readonly IVisitorWriteRepository _visitorWriteRepository;
        private readonly IVisitWriteRepository _visitWriteRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper,
                          UserManager<User> userManager, RoleManager<Role> roleManager, PrisonDbContext prisonDbContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _staffWriteRepository = _unitOfWork.GetRepository<IStaffWriteRepository>();
            _prisonStaffWriteRepository = _unitOfWork.GetRepository<IPrisonStaffWriteRepository>();
            _scheduleWriteRepository = _unitOfWork.GetRepository<IScheduleWriteRepository>();
            _visitorWriteRepository = _unitOfWork.GetRepository<IVisitorWriteRepository>();
            _visitWriteRepository = _unitOfWork.GetRepository<IVisitWriteRepository>();
        }

        public async Task<GenericResponseModel<bool>> CreateStaffUserAsync(RegisterStaffUserDto registerStaffUserDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userExists = await _userManager.FindByEmailAsync(registerStaffUserDto.CreateUserDto.Email);
                if (userExists != null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "A user with this email already exists",
                        (int)HttpStatusCode.BadRequest);
                }

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = registerStaffUserDto.CreateUserDto.UserName,
                    FirstName = registerStaffUserDto.CreateUserDto.FirstName,
                    LastName = registerStaffUserDto.CreateUserDto.LastName,
                    Email = registerStaffUserDto.CreateUserDto.Email,
                    PhoneNumber = registerStaffUserDto.CreateUserDto.PhoneNumber,
                    BirthDate = registerStaffUserDto.CreateUserDto.BirthDate
                };

                var createUserResult = await _userManager.CreateAsync(user, registerStaffUserDto.CreateUserDto.Password);
                if (!createUserResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return GenericResponseModel<bool>.FailureResponse(
                        $"An error occurred while creating user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}",
                        (int)HttpStatusCode.InternalServerError);
                }

                var staffEntity = new Staff
                {
                    Id = Guid.NewGuid(),
                    Name = $"{user.FirstName} {user.LastName}",
                    DateOfStarting = registerStaffUserDto.CreateStaffDto.DateOfStarting,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    UserId = user.Id,
                    Schedules = new List<Schedule>(),
                    PrisonStaffs = new List<PrisonStaff>()
                };

                await _staffWriteRepository.AddAsync(staffEntity);

                var prisonStaff = new PrisonStaff
                {
                    Id = Guid.NewGuid(),
                    StaffId = staffEntity.Id,
                    PrisonId = registerStaffUserDto.CreateStaffDto.PrisonId
                };
                await _prisonStaffWriteRepository.AddAsync(prisonStaff);

                foreach (var schedule in registerStaffUserDto.CreateStaffDto.Schedules)
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
                    true,
                    (int)HttpStatusCode.Created,
                    "User and Staff created successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create staff user. Error: {ErrorMessage}", ex.Message);
                await _unitOfWork.RollbackTransactionAsync();
                return GenericResponseModel<bool>.FailureResponse(
                    $"An error occurred while creating user and staff: {ex.Message}",
                    (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResponseModel<bool>> RegisterVisitorWithUserAsync(RegisterVisitorUserDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if (dto.CreateVisitorDto.RelationToPrisoner != Relationship.Lawyer &&
                    dto.CreateVisitorDto.RelationToPrisoner != Relationship.Official &&
                    dto.CreateVisitorDto.RelationToPrisoner != Relationship.SocialWorker)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "User registration only allowed for Lawyers and Officials",
                        (int)HttpStatusCode.BadRequest);
                }

                var user = new User
                {
                    UserName = dto.CreateUserDto.UserName,
                    Email = dto.CreateUserDto.Email,
                    FirstName = dto.CreateUserDto.FirstName,
                    LastName = dto.CreateUserDto.LastName,
                    PhoneNumber = dto.CreateUserDto.PhoneNumber,
                    BirthDate = dto.CreateUserDto.BirthDate,
                    Id = Guid.NewGuid().ToString()
                };

                var userResult = await _userManager.CreateAsync(user, dto.CreateUserDto.Password);
                if (!userResult.Succeeded)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        string.Join(", ", userResult.Errors.Select(e => e.Description)),
                        (int)HttpStatusCode.BadRequest);
                }

                var visitor = new Visitor
                {
                    UserId = user.Id,
                    Id = Guid.NewGuid(),
                    Name = dto.CreateVisitorDto.Name,
                    PhoneNumber = dto.CreateVisitorDto.PhoneNumber,
                    RelationToPrisoner = dto.CreateVisitorDto.RelationToPrisoner,
                };

                await _visitorWriteRepository.AddAsync(visitor);

                if (dto.CreateVisitorDto.Visits != null)
                {
                    var visit = new Visit
                    {
                        VisitorId = visitor.Id,
                        PrisonerId = dto.CreateVisitorDto.Visits.PrisonerId,
                        VisitDate = dto.CreateVisitorDto.Visits.VisitDate,
                        DurationInMinutes = dto.CreateVisitorDto.Visits.DurationInMinutes,
                        VisitType = dto.CreateVisitorDto.Visits.VisitType
                    };

                    await _visitWriteRepository.AddAsync(visit);
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    true,
                    (int)HttpStatusCode.Created,
                    "Visitor and User registered successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Log.Error(ex, "Error registering visitor with user");
                return GenericResponseModel<bool>.FailureResponse(
                    $"Error: {ex.Message}",
                    (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<GenericResponseModel<CreateUserDto>> CreateUserAsync(CreateUserDto model)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.BirthDate
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GenericResponseModel<CreateUserDto>.FailureResponse(
                    string.Join(" \n ", result.Errors.Select(e => $"{e.Code} - {e.Description}")),
                    (int)HttpStatusCode.BadRequest);
            }

            var defaultRole = "Admin"; 
            var roleResult = await _userManager.AddToRoleAsync(user, defaultRole);

            if (!roleResult.Succeeded)
            {
                return GenericResponseModel<CreateUserDto>.FailureResponse(
                    string.Join(" \n ", roleResult.Errors.Select(e => $"{e.Code} - {e.Description}")),
                    (int)HttpStatusCode.BadRequest);
            }

            return GenericResponseModel<CreateUserDto>.SuccessResponse(
                model,
                (int)HttpStatusCode.Created,
                "User created successfully.");
        }


        public async Task<GenericResponseModel<List<GetUserDto>>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
                var usersDto = _mapper.Map<List<GetUserDto>>(users);

            return GenericResponseModel<List<GetUserDto>>.SuccessResponse(
                usersDto,
                (int)HttpStatusCode.OK,
                "Users retrieved successfully.");
                    
           
        }

        public async Task<GenericResponseModel<bool>> DeleteUserAsync(string userIdOrName)
        {
            
                if (string.IsNullOrWhiteSpace(userIdOrName))
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "User ID or Name is required.",
                        (int)HttpStatusCode.BadRequest);
                }

                var user = await _userManager.FindByIdAsync(userIdOrName) ??
                          await _userManager.FindByNameAsync(userIdOrName);

                if (user == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "User not found.",
                        (int)HttpStatusCode.NotFound);
                }

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        string.Join(" \n ", result.Errors.Select(e => $"{e.Code} - {e.Description}")),
                        (int)HttpStatusCode.BadRequest);
                }

                return GenericResponseModel<bool>.SuccessResponse(
                    true,
                    (int)HttpStatusCode.OK,
                    "User deleted successfully.");
          
        }

        public async Task<GenericResponseModel<bool>> AssignRoleToUserAsync(string userId, string[] roles)
        {
             if (string.IsNullOrWhiteSpace(userId) || roles == null || !roles.Any())
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "User ID and roles are required.",
                        (int)HttpStatusCode.BadRequest);
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "User not found.",
                        (int)HttpStatusCode.NotFound);
                }

                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        return GenericResponseModel<bool>.FailureResponse(
                            $"Role '{role}' does not exist.",
                            (int)HttpStatusCode.BadRequest);
                    }
                }

                var result = await _userManager.AddToRolesAsync(user, roles);

                if (!result.Succeeded)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        string.Join(" \n ", result.Errors.Select(e => $"{e.Code} - {e.Description}")),
                        (int)HttpStatusCode.BadRequest);
                }

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                (int)HttpStatusCode.OK,
                "Roles assigned successfully.");
                    
           
        }

        public async Task<GenericResponseModel<bool>> UpdateUserAsync(UpdateUserDto model)
        {  var user = await _userManager.FindByIdAsync(model.UserId);

                if (user == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "User not found.",
                        (int)HttpStatusCode.NotFound);
                }

                user.BirthDate = model.BirthDate;
                user.PhoneNumber = model.PhoneNumber;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        string.Join(" \n ", result.Errors.Select(e => $"{e.Code} - {e.Description}")),
                        (int)HttpStatusCode.BadRequest);
                }

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                (int)HttpStatusCode.OK,
                "User updated successfully.");
                    
          
        }

        public async Task<GenericResponseModel<GetUserDto>> GetUserByIdAsync(string userIdOrName)
        {
           
                if (string.IsNullOrWhiteSpace(userIdOrName))
                {
                    return GenericResponseModel<GetUserDto>.FailureResponse(
                        "User ID or Name is required.",
                        (int)HttpStatusCode.BadRequest);
                }

                var user = await _userManager.FindByIdAsync(userIdOrName) ??
                          await _userManager.FindByNameAsync(userIdOrName);

                if (user == null)
                {
                    return GenericResponseModel<GetUserDto>.FailureResponse(
                        "User not found.",
                        (int)HttpStatusCode.NotFound);
                }

                var userDto = _mapper.Map<GetUserDto>(user);
            return GenericResponseModel<GetUserDto>.SuccessResponse(
                userDto,
                (int)HttpStatusCode.OK,
                "User retrieved successfully.");
                    
           
        }
    }
}