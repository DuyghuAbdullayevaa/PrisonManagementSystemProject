using AutoMapper;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DTOs;

namespace PrisonManagementSystem.BL.Mappings
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<Schedule, GetScheduleDto>().ReverseMap();
            CreateMap<CreateScheduleDto, Schedule>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())).ReverseMap();
            CreateMap<UpdateScheduleDto, Schedule>();
        }
    }
}
