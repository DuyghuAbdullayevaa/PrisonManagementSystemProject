using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentPunishmentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerIncidentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerPunishmentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPunishmentRepository;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serilog;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class PunishmentService : IPunishmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPunishmentReadRepository _punishmentReadRepository;
        private readonly IPunishmentWriteRepository _punishmentWriteRepository;
        private readonly IIncidentReadRepository _incidentReadRepository;
        private readonly IPrisonerIncidentReadRepository _prisonerIncidentReadRepository;
        private readonly IIncidentPunishmentWriteRepository _incidentPunishmentWriteRepository;
        private readonly IIncidentPunishmentReadRepository _incidentPunishmentReadRepository;
        private readonly IPrisonerPunishmentReadRepository _prisonerPunishmentReadRepository;
        private readonly IPrisonerPunishmentWriteRepository _prisonerPunishmentWriteRepository;
        private readonly IPrisonerReadRepository _prisonerReadRepository;
        private readonly IPrisonerWriteRepository _prisonerWriteRepository;

        public PunishmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _punishmentReadRepository = _unitOfWork.GetRepository<IPunishmentReadRepository>();
            _punishmentWriteRepository = _unitOfWork.GetRepository<IPunishmentWriteRepository>();
            _incidentReadRepository = _unitOfWork.GetRepository<IIncidentReadRepository>();
            _prisonerIncidentReadRepository = _unitOfWork.GetRepository<IPrisonerIncidentReadRepository>();
            _prisonerReadRepository = _unitOfWork.GetRepository<IPrisonerReadRepository>();
            _incidentPunishmentWriteRepository = _unitOfWork.GetRepository<IIncidentPunishmentWriteRepository>();
            _incidentPunishmentReadRepository = _unitOfWork.GetRepository<IIncidentPunishmentReadRepository>();
            _prisonerPunishmentReadRepository = _unitOfWork.GetRepository<IPrisonerPunishmentReadRepository>();
            _prisonerWriteRepository = _unitOfWork.GetRepository<IPrisonerWriteRepository>();
            _prisonerPunishmentWriteRepository = _unitOfWork.GetRepository<IPrisonerPunishmentWriteRepository>();
        }

        public async Task<GenericResponseModel<bool>> CreatePunishmentAsync(CreatePunishmentDto createPunishmentDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var validationResult = await ValidateCreatePunishmentRequest(createPunishmentDto);
                if (!validationResult.IsValid)
                {
                    return validationResult.ErrorResponse;
                }

                var punishments = MapAndPreparePunishments(createPunishmentDto);
                await _punishmentWriteRepository.AddRangeAsync(punishments);

                var prisonerPunishments = CreatePrisonerPunishments(punishments, createPunishmentDto);
                if (prisonerPunishments.Count == 0)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "No valid prisoners found for this punishment",
                        statusCode: 400
                    );
                }

                await _prisonerPunishmentWriteRepository.AddRangeAsync(prisonerPunishments);

                await ProcessExtendedSentences(punishments, createPunishmentDto.IncidentId);

                var incidentPunishments = CreateIncidentPunishments(punishments, createPunishmentDto.IncidentId);
                await _incidentPunishmentWriteRepository.AddRangeAsync(incidentPunishments);

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 201,
                    message: "Punishments created successfully"
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                Log.Error(ex, "Error creating punishments for IncidentId: {IncidentId}", createPunishmentDto.IncidentId);
                return GenericResponseModel<bool>.FailureResponse(
                    error: $"Error creating punishments: {ex.Message}",
                    statusCode: 500
                );
            }
        }

        public async Task<GenericResponseModel<bool>> UpdatePunishmentAsync(Guid punishmentId, UpdatePunishmentDto updatePunishmentDto)
        {
           
                var punishment = await _punishmentReadRepository.GetByIdAsync(punishmentId);
                if (punishment == null)
                {
                    return PunishmentNotFoundResponse();
                }

                var validationResult = await ValidateUpdatePunishmentRequest(updatePunishmentDto, punishment);
                if (!validationResult.IsValid)
                {
                    return validationResult.ErrorResponse;
                }

                await UpdateIncidentPunishmentRelationship(punishment, updatePunishmentDto.IncidentId);
                await UpdatePrisonerPunishmentRelationship(punishment, updatePunishmentDto.PrisonerId);

                punishment.Type = updatePunishmentDto.PunishmentType;
                _mapper.Map(updatePunishmentDto, punishment);

                await _punishmentWriteRepository.UpdateAsync(punishment);
                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: "Punishment updated successfully"
                );
           
        }

        public async Task<GenericResponseModel<GetPunishmentDto>> GetPunishmentByIdAsync(Guid punishmentId)
        {
          
                var punishment = await GetPunishmentWithDetails(punishmentId);
                if (punishment == null)
                {
                    return PunishmentNotFoundResponse<GetPunishmentDto>();
                }

                return GenericResponseModel<GetPunishmentDto>.SuccessResponse(
                    data: _mapper.Map<GetPunishmentDto>(punishment),
                    statusCode: 200,
                    message: "Punishment retrieved successfully"
                );
         
        }

        public async Task<GenericResponseModel<PaginationResponse<GetPunishmentDto>>> GetAllPunishmentsAsync(PaginationRequest paginationRequest)
        {
              var punishments = await _punishmentReadRepository.GetAllByPagingAsync(
                    predicate: null,
                    include: query => query
                        .Include(p => p.PrisonerPunishments)
                            .ThenInclude(pi => pi.Prisoner)
                        .Include(p => p.IncidentPunishments)
                            .ThenInclude(ip => ip.Incident),
                    orderBy: q => q.OrderByDescending(p => p.StartDate),
                    currentPage: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize
                );

                int totalCount = await _punishmentReadRepository.GetCountAsync();
                var punishmentDtos = _mapper.Map<List<GetPunishmentDto>>(punishments);

                return GenericResponseModel<PaginationResponse<GetPunishmentDto>>.SuccessResponse(
                    data: new PaginationResponse<GetPunishmentDto>(totalCount, punishmentDtos, paginationRequest.PageNumber, paginationRequest.PageSize),
                    statusCode: 200,
                    message: "Punishments retrieved successfully"
                );
           
        }

        public async Task<GenericResponseModel<bool>> DeletePunishmentAsync(Guid punishmentId, bool isHardDelete)
        {
           
                var punishment = await GetPunishmentWithIncidentRelationships(punishmentId);
                if (punishment == null)
                {
                    return PunishmentNotFoundResponse();
                }

                var result = await _punishmentWriteRepository.DeleteAsync(punishment, isHardDelete);
                if (!result)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        error: "Error deleting punishment",
                        statusCode: 400
                    );
                }

                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    data: true,
                    statusCode: 200,
                    message: $"Punishment {(isHardDelete ? "permanently deleted" : "archived")} successfully"
                );
          
        }

        #region Private Methods

        private async Task UpdateReleaseDateIfNeeded(Punishment punishment, IncidentType incidentType)
        {
            if (punishment.Type == PunishmentType.ExtendedSentence)
            {
                int releaseDateIncreaseDays = GetReleaseDateIncreaseDays(incidentType);
                await UpdatePrisonersReleaseDates(punishment.Id, releaseDateIncreaseDays);
                await _unitOfWork.CommitAsync();
            }
        }

        private int GetReleaseDateIncreaseDays(IncidentType incidentType)
        {
            return incidentType switch
            {
                IncidentType.DisciplinaryAction => 20,
                IncidentType.Assault => 45,
                IncidentType.EscapeAttempt => 90,
                IncidentType.DrugUse => 30,
                IncidentType.Violence => 60,
                IncidentType.PropertyDamage => 25,
                IncidentType.Threat => 35,
                IncidentType.Death => 120,
                _ => 30
            };
        }

        private async Task UpdatePrisonersReleaseDates(Guid punishmentId, int daysToAdd)
        {
            var prisonerPunishments = await _prisonerPunishmentReadRepository.GetAllAsync(
                pp => pp.PunishmentId == punishmentId);

            foreach (var prisonerPunishment in prisonerPunishments)
            {
                var prisoner = await _prisonerReadRepository.GetByIdAsync(prisonerPunishment.PrisonerId);
                if (prisoner?.ReleaseDate.HasValue == true)
                {
                    prisoner.ReleaseDate = prisoner.ReleaseDate.Value.AddDays(daysToAdd);
                    await _prisonerWriteRepository.UpdateAsync(prisoner);
                }
            }
        }

        private async Task<(bool IsValid, GenericResponseModel<bool> ErrorResponse)> ValidateCreatePunishmentRequest(CreatePunishmentDto createPunishmentDto)
        {
            var incident = await _incidentReadRepository.GetByIdAsync(createPunishmentDto.IncidentId);
            if (incident == null)
            {
                return (false, GenericResponseModel<bool>.FailureResponse(
                    error: "Incident not found",
                    statusCode: 404
                ));
            }

            return (true, null);
        }

        private List<Punishment> MapAndPreparePunishments(CreatePunishmentDto createPunishmentDto)
        {
            return createPunishmentDto.Punishments.Select(punishmentDto =>
            {
                var punishment = _mapper.Map<Punishment>(punishmentDto);
                punishment.Type = punishmentDto.PunishmentType;
                punishment.PrisonerPunishments = new List<PrisonerPunishment>();
                return punishment;
            }).ToList();
        }

        private List<PrisonerPunishment> CreatePrisonerPunishments(List<Punishment> punishments, CreatePunishmentDto createPunishmentDto)
        {
            return punishments
                .SelectMany(punishment => createPunishmentDto.Punishments
                    .Select(p => new PrisonerPunishment
                    {
                        Id = Guid.NewGuid(),
                        PrisonerId = p.PrisonerId,
                        PunishmentId = punishment.Id
                    })
                ).ToList();
        }
        private async Task ProcessExtendedSentences(List<Punishment> punishments, Guid incidentId)
        {
            var incident = await _incidentReadRepository.GetByIdAsync(incidentId);
            foreach (var punishment in punishments)
            {
                await UpdateReleaseDateIfNeeded(punishment, incident.Type);
            }
        }

        private List<IncidentPunishment> CreateIncidentPunishments(List<Punishment> punishments, Guid incidentId)
        {
            return punishments.Select(punishment => new IncidentPunishment
            {
                IncidentId = incidentId,
                PunishmentId = punishment.Id
            }).ToList();
        }

        private GenericResponseModel<bool> PunishmentNotFoundResponse()
        {
            return GenericResponseModel<bool>.FailureResponse(
                error: "Punishment not found",
                statusCode: 404
            );
        }

        private GenericResponseModel<T> PunishmentNotFoundResponse<T>() where T : class
        {
            return GenericResponseModel<T>.FailureResponse(
                error: "Punishment not found",
                statusCode: 404
            );
        }

        private async Task<(bool IsValid, GenericResponseModel<bool> ErrorResponse)> ValidateUpdatePunishmentRequest(
            UpdatePunishmentDto updatePunishmentDto, Punishment punishment)
        {
            var prisonerExists = await _prisonerReadRepository.AnyAsync(p => p.Id == updatePunishmentDto.PrisonerId);
            if (!prisonerExists)
            {
                return (false, GenericResponseModel<bool>.FailureResponse(
                    error: "Prisoner not found",
                    statusCode: 404
                ));
            }

            var incidentExists = await _incidentReadRepository.AnyAsync(i => i.Id == updatePunishmentDto.IncidentId);
            if (!incidentExists)
            {
                return (false, GenericResponseModel<bool>.FailureResponse(
                    error: "Incident not found",
                    statusCode: 404
                ));
            }

            var isPrisonerInIncident = await _prisonerIncidentReadRepository.AnyAsync(ip =>
                ip.IncidentId == updatePunishmentDto.IncidentId && ip.PrisonerId == updatePunishmentDto.PrisonerId);

            if (!isPrisonerInIncident)
            {
                return (false, GenericResponseModel<bool>.FailureResponse(
                    error: "Prisoner not part of specified incident",
                    statusCode: 400
                ));
            }

            if (updatePunishmentDto.EndDate.HasValue && updatePunishmentDto.StartDate >= updatePunishmentDto.EndDate.Value)
            {
                return (false, GenericResponseModel<bool>.FailureResponse(
                    error: "Start date must be before end date",
                    statusCode: 400
                ));
            }

            return (true, null);
        }

        private async Task UpdateIncidentPunishmentRelationship(Punishment punishment, Guid newIncidentId)
        {
            var existingIncidentPunishment = await _incidentPunishmentReadRepository
                .GetSingleAsync(ip => ip.PunishmentId == punishment.Id);

            if (existingIncidentPunishment?.IncidentId != newIncidentId)
            {
                if (existingIncidentPunishment != null)
                {
                    await _incidentPunishmentWriteRepository.SoftDeleteAsync(existingIncidentPunishment);
                }

                await _incidentPunishmentWriteRepository.AddAsync(new IncidentPunishment
                {
                    IncidentId = newIncidentId,
                    PunishmentId = punishment.Id
                });
            }
        }

        private async Task UpdatePrisonerPunishmentRelationship(Punishment punishment, Guid newPrisonerId)
        {
            var existingPrisonerPunishment = await _prisonerPunishmentReadRepository
                .GetSingleAsync(pp => pp.PunishmentId == punishment.Id);

            if (existingPrisonerPunishment?.PrisonerId != newPrisonerId)
            {
                if (existingPrisonerPunishment != null)
                {
                    await _prisonerPunishmentWriteRepository.SoftDeleteAsync(existingPrisonerPunishment);
                }

                await _prisonerPunishmentWriteRepository.AddAsync(new PrisonerPunishment
                {
                    PrisonerId = newPrisonerId,
                    PunishmentId = punishment.Id
                });
            }
        }

        private async Task<Punishment> GetPunishmentWithDetails(Guid punishmentId)
        {
            return await _punishmentReadRepository.GetSingleAsync(
                p => p.Id == punishmentId,
                include: query => query
                    .Include(p => p.PrisonerPunishments)
                        .ThenInclude(pi => pi.Prisoner)
                    .Include(p => p.IncidentPunishments)
                        .ThenInclude(ip => ip.Incident),
                enableTracking: false
            );
        }

        private async Task<Punishment> GetPunishmentWithIncidentRelationships(Guid punishmentId)
        {
            return await _punishmentReadRepository.GetSingleAsync(
                p => p.Id == punishmentId,
                include: query => query.Include(p => p.IncidentPunishments)
            );
        }

        #endregion
    }
}