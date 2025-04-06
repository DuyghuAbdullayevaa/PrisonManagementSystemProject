using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using System.Linq;

namespace PrisonManagementSystem.BL.Profiles
{
    public class PrisonerProfile : Profile
    {
        public PrisonerProfile()
        {
            CreateMap<Prisoner, GetPrisonerDto>()
                  .ForMember(dest => dest.CellNumber, opt => opt.MapFrom(src => src.Cell.CellNumber))
                  .ForMember(dest => dest.Crimes, opt => opt.MapFrom(src => src.PrisonerCrimes.Select(pc => pc.Crime).ToList()))
                  .ForMember(dest => dest.Incidents, opt => opt.MapFrom(src => src.PrisonersIncidents.Select(pi => pi.Incident).ToList()))
                  .ForMember(dest => dest.Punishments, opt => opt.MapFrom(src => src.PrisonerPunishments.Select(pi => pi.Punishment).ToList()))
                  .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())).ReverseMap();

            CreateMap<CreatePrisonerDto, Prisoner>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.AdmissionDate, opt => opt.MapFrom(src => DateTime.UtcNow)).ReverseMap();

            CreateMap<UpdatePrisonerDto, Prisoner>().ReverseMap();
        }
    }
}
