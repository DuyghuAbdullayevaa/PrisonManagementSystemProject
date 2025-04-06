using AutoMapper;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.DTOs.Visit;

namespace PrisonManagementSystem.BL.AutoMapperProfiles
{
    public class VisitProfile : Profile
    {
        public VisitProfile()
        {
            CreateMap<Visit, GetVisitDto>()
                .ForMember(dest => dest.PrisonerName, opt => opt.MapFrom(src => src.Prisoner.FirstName))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.DurationInMinutes))
                .ReverseMap();

            CreateMap<Visit, UpdateVisitDto>().ReverseMap();
            CreateMap<Visit, CreateVisitDto>().ReverseMap();
        }
    }
}
