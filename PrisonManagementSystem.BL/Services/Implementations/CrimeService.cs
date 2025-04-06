using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICrimeRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerCrimeRepository;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Models;

namespace PrisonManagementSystem.BL.Services.Implementations
{
    public class CrimeService : ICrimeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICrimeReadRepository _crimeReadRepository;
        private readonly ICrimeWriteRepository _crimeWriteRepository;
        private readonly IPrisonerReadRepository _prisonerReadRepository;
        private readonly IPrisonerCrimeWriteRepository _prisonerCrimeWriteRepository;
    

        public CrimeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crimeReadRepository = _unitOfWork.GetRepository<ICrimeReadRepository>();
            _crimeWriteRepository = _unitOfWork.GetRepository<ICrimeWriteRepository>();
            _prisonerReadRepository = _unitOfWork.GetRepository<IPrisonerReadRepository>();
            _prisonerCrimeWriteRepository = _unitOfWork.GetRepository<IPrisonerCrimeWriteRepository>();
     
        }

        public async Task<GenericResponseModel<CreateCrimeDto>> CreateCrimeAsync(CreateCrimeDto createCrimeDto)
        {
            await _unitOfWork.BeginTransactionAsync();

          
                var crime = new Crime
                {
                    Details = createCrimeDto.Details,
                    SeverityLevel = createCrimeDto.SeverityLevel,
                    Type = createCrimeDto.Type
                };

                await _crimeWriteRepository.AddAsync(crime);


                foreach (var prisonerDto in createCrimeDto.Prisoners)
                {
                    var prisoner = await _prisonerReadRepository
                        .GetSingleAsync(p => p.FirstName == prisonerDto.FirstName);

                    if (prisoner != null)
                    {
                        var prisonerCrime = new PrisonerCrime
                        {
                            PrisonerId = prisoner.Id,
                            CrimeId = crime.Id
                        };

                        await _prisonerCrimeWriteRepository.AddAsync(prisonerCrime);
                    }
                }

                await _unitOfWork.CommitAsync(); 
                await _unitOfWork.CommitTransactionAsync(); 

                return GenericResponseModel<CreateCrimeDto>.SuccessResponse(createCrimeDto, 201, "Crime and prisoners added successfully.");
           
        }


        public async Task<GenericResponseModel<PaginationResponse<GetCrimeDto>>> GetAllCrimesAsync(PaginationRequest paginationRequest)
        {
            
                var crimes = await _crimeReadRepository.GetAllByPagingAsync(
                    currentPage: paginationRequest.PageNumber,
                    pageSize: paginationRequest.PageSize,
                    include: crime => crime.Include(c => c.PrisonerCrimes).ThenInclude(pc => pc.Prisoner)
                );

                var totalCount = await _crimeReadRepository.GetCountAsync();
                var crimeDtos = _mapper.Map<List<GetCrimeDto>>(crimes);
                var paginatedResponse = new PaginationResponse<GetCrimeDto>(totalCount, crimeDtos, paginationRequest.PageNumber, paginationRequest.PageSize);

                return GenericResponseModel<PaginationResponse<GetCrimeDto>>.SuccessResponse(
                    paginatedResponse,
                    200,
                    "Bütün cinayətlər uğurla əldə edildi."
                );
            
           
        }

        public async Task<GenericResponseModel<UpdateCrimeDto>> UpdateCrimeAsync(Guid crimeId, UpdateCrimeDto updateCrimeDto)
        {
           
                var crimeEntity = await _crimeReadRepository.GetByIdAsync(crimeId);
                if (crimeEntity == null)
                {
                    return GenericResponseModel<UpdateCrimeDto>.FailureResponse(
                        "Cinayət tapılmadı.",
                        404
                    );
                }

                _mapper.Map(updateCrimeDto, crimeEntity);
                await _crimeWriteRepository.UpdateAsync(crimeEntity);
                await _unitOfWork.CommitAsync();

                return GenericResponseModel<UpdateCrimeDto>.SuccessResponse(
                    _mapper.Map<UpdateCrimeDto>(crimeEntity),
                    200,
                    "Cinayət uğurla yeniləndi."
                );
            
        
        }

        public async Task<GenericResponseModel<GetCrimeDto>> GetCrimeByIdAsync(Guid crimeId)
        {
           
                var crimeEntity = await _crimeReadRepository.GetSingleAsync(
                    predicate: c => c.Id == crimeId,
                    include: q => q.Include(c => c.PrisonerCrimes)
                        .ThenInclude(pc => pc.Prisoner)
                );

                if (crimeEntity == null)
                {
                    return GenericResponseModel<GetCrimeDto>.FailureResponse(
                        "Cinayət tapılmadı.",
                        404
                    );
                }

                return GenericResponseModel<GetCrimeDto>.SuccessResponse(
                    _mapper.Map<GetCrimeDto>(crimeEntity),
                    200,
                    "Cinayət uğurla əldə edildi."
                );
          
        }

        public async Task<GenericResponseModel<GetCrimeDto>> GetCrimeWithPrisonersAsync(Guid crimeId)
        {
            
                var crimeEntity = await _crimeReadRepository.GetSingleAsync(
                    c => c.Id == crimeId,
                    include: query => query
                        .Include(c => c.PrisonerCrimes)
                        .ThenInclude(pc => pc.Prisoner),
                    enableTracking: false
                );

                if (crimeEntity == null)
                {
                    return GenericResponseModel<GetCrimeDto>.FailureResponse(
                        "Cinayət tapılmadı.",
                        404
                    );
                }

                return GenericResponseModel<GetCrimeDto>.SuccessResponse(
                    _mapper.Map<GetCrimeDto>(crimeEntity),
                    200,
                    "Cinayət məhbuslarla birlikdə uğurla əldə edildi."
                );
          
        }

        public async Task<GenericResponseModel<bool>> DeleteCrimeAsync(Guid crimeId, bool isHardDelete)
        {
             var crimeEntity = await _crimeReadRepository.GetSingleAsync(
                    c => c.Id == crimeId,
                    include: query => query
                        .Include(c => c.PrisonerCrimes)
                        .ThenInclude(pc => pc.Prisoner),
                    enableTracking: false
                );

                if (crimeEntity == null)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Cinayət tapılmadı.",
                        404
                    );
                }

                if (crimeEntity.PrisonerCrimes.Any())
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Bu cinayətə bağlı məhbuslar var. Silinə bilməz.",
                        400
                    );
                }

                var result = await _crimeWriteRepository.DeleteAsync(crimeEntity, isHardDelete);
                if (!result)
                {
                    return GenericResponseModel<bool>.FailureResponse(
                        "Cinayət silinərkən xəta baş verdi.",
                        400
                    );
                }

                await _unitOfWork.CommitAsync();

                return GenericResponseModel<bool>.SuccessResponse(
                    true,
                    200,
                    "Cinayət uğurla silindi."
                );
            
            
        }
    }
}