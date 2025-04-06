using AutoMapper;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.DTOs.Punishment;
using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.BL.MappingProfiles
{
    public class PunishmentProfile : Profile
    {
        public PunishmentProfile()
        {
            CreateMap<PunishmentAssignmentDto, Punishment>()
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate));

            CreateMap<Punishment, CreatePunishmentDto>().ReverseMap();

            CreateMap<Punishment, GetPunishmentDto>()
                .ForMember(dest => dest.IncidentDescriptions, opt => opt.MapFrom(src => src.IncidentPunishments.Select(ip => ip.Incident.Description)))
                .ForMember(dest => dest.PrisonerName, opt => opt.MapFrom(src => src.PrisonerPunishments.Select(ip => ip.Prisoner.FirstName).ToList()));

            CreateMap<UpdatePunishmentDto, Punishment>()
                .ForMember(dest => dest.IncidentPunishments, opt => opt.Ignore());
        }
    }
}
