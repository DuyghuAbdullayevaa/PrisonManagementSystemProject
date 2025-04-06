using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using System.Linq;

namespace PrisonManagementSystem.BL.MappingProfiles
{
    public class CrimeProfile : Profile
    {
        public CrimeProfile()
        {
            CreateMap<Crime, GetCrimeDto>()
                .ForMember(dest => dest.Prisoners, opt => opt.MapFrom(src =>
                    src.PrisonerCrimes.Select(pc => new PrisonerDto
                    { FirstName = pc.Prisoner.FirstName, })))
                .ReverseMap();

            CreateMap<Crime, CreateCrimeDto>()
                .ForMember(dest => dest.Prisoners, opt => opt.MapFrom(src =>
                    src.PrisonerCrimes)).ReverseMap();

            CreateMap<UpdateCrimeDto, Crime>().ReverseMap();
        }
    }
}
