using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.DTOs.Visitor;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DTOs;

namespace PrisonManagementSystem.BL.Mappings
{
    public class VisitorProfile : Profile
    {
        public VisitorProfile()
        {
            CreateMap<Visitor, GetVisitorDto>()
                .ForMember(dest => dest.Visits, opt => opt.MapFrom(src => src.VisitHistory))
                .ReverseMap();

            CreateMap<UpdateVisitorDto, Visitor>();

            CreateMap<CreateVisitorDto, Visitor>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)) // Map Name
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber)) // Map PhoneNumber
                .ReverseMap();
        }
    }
}
