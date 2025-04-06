using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Staff;
using PrisonManagementSystem.DTOs;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.BL.DTOs.Prison;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Entities.Prison;

public class StaffProfile : Profile
{
    public StaffProfile()
    {
        // Mapping for updating a staff member
        CreateMap<UpdateStaffDto, Staff>();

        CreateMap<Staff, GetStaffDto>()
            // Map DateOfStarting from Staff to DateOfJoining in GetStaffDto
            .ForMember(dest => dest.DateOfJoining, opt => opt.MapFrom(src => src.DateOfStarting))

            // Map Prisons from Staff to GetStaffDto (prison names)
            .ForMember(dest => dest.Prisons, opt => opt.MapFrom(src =>
                src.PrisonStaffs.Select(pc => pc.Prison.Name).ToList()))

            // Map Schedules from Staff to GetStaffDto
            .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules))

            // Reverse map
            .ReverseMap();

        // Mapping for creating a staff member
        CreateMap<CreateStaffDto, Staff>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid())) // Create new Id
            .ForMember(dest => dest.PrisonStaffs, opt => opt.MapFrom(src => new List<PrisonStaff>())) // Empty PrisonStaffs list
            .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => new List<Schedule>())); // Empty Schedules list

        // Reverse map (optional)
        CreateMap<Staff, CreateStaffDto>().ReverseMap();
    }
}
