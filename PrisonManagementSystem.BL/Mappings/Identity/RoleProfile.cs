using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Identiity.Role;
using PrisonManagementSystem.DAL.Entities.Identity;

namespace PrisonManagementSystem.BL.Mappings.Identity
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, GetRoleDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))  // No conversion needed
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<CreateRoleDto, Role>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();

            CreateMap<UpdateRoleDto, Role>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
        }
    }
}
