using AutoMapper;
using PrisonManagementSystem.BL.DTOs.Identiity.User;
using PrisonManagementSystem.DAL.Entities.Identity;

namespace PrisonManagementSystem.BL.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserDto>().ReverseMap();

            CreateMap<CreateUserDto, User>().ReverseMap();


            CreateMap<UpdateUserDto, User>().ReverseMap();
               
        }
    }
}
