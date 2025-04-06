using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.BL.Profiles
{
    public class IncidentProfile : Profile
    {
        public IncidentProfile()
        {
            // Mapping for CreateIncidentDto
            CreateMap<CreateIncidentDto, Incident>()
                .ForMember(dest => dest.PrisonerIncidents, opt => opt.Ignore())
                .ForMember(dest => dest.IncidentPunishments, opt => opt.Ignore())
                .ForMember(dest => dest.IncidentCells, opt => opt.Ignore())
                .ForMember(dest => dest.Reports, opt => opt.Ignore());

            // Mapping for UpdateIncidentDto
            CreateMap<UpdateIncidentDto, Incident>()
                .ForMember(dest => dest.PrisonerIncidents, opt => opt.Ignore())
                .ForMember(dest => dest.IncidentPunishments, opt => opt.Ignore())
                .ForMember(dest => dest.IncidentCells, opt => opt.Ignore())
                .ForMember(dest => dest.Reports, opt => opt.Ignore());
          CreateMap<Incident, GetIncidentDto>()
                .ForMember(dest => dest.PrisonName, opt => opt.MapFrom(src => src.Prison.Name))  // Map PrisonName
                .ForMember(dest => dest.Prisoners, opt => opt.MapFrom(src => src.PrisonerIncidents
                    .Select(pi => pi.Prisoner.FirstName + " " + pi.Prisoner.LastName).ToList())) // Map Prisoners
                .ForMember(dest => dest.Cells, opt => opt.MapFrom(src => src.IncidentCells
                    .Select(ic => ic.Cell.CellNumber).ToList()))  // Map Cells
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))  // Map Description
                .ForMember(dest => dest.IncidentDate, opt => opt.MapFrom(src => src.IncidentDate))  // Map IncidentDate
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))  // Map Status
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));   
        }
    }
}
