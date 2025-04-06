using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Prison;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.BL.Mappings
{
    public class PrisonProfile : Profile
    {
        public PrisonProfile()
        {
            CreateMap<CreatePrisonDto, Prison>().ReverseMap();
            CreateMap<UpdatePrisonDto, Prison>().ReverseMap();

            CreateMap<Prison, GetPrisonDto>()
                .ForMember(dest => dest.Cells, opt => opt.MapFrom(src => src.Cells))
                .ForMember(dest => dest.Staffs, opt => opt.MapFrom(src => src.PrisonStaffs.Select(ps => ps.Staff)))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status == PrisonStatus.Active))
                .ForMember(dest => dest.Incidents, opt => opt.MapFrom(src => src.Incidents))
                .ReverseMap();
        }
    }
}
