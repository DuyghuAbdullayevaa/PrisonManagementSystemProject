using PrisonManagementSystem.BL.DTOs.Identiity.User;
using PrisonManagementSystem.BL.DTOs.ResponseModel;

public interface IUserService
{
    Task<GenericResponseModel<bool>> AssignRoleToUserAsync(string userId, string[] roles);
    Task<GenericResponseModel<bool>> CreateStaffUserAsync(RegisterStaffUserDto registerStaffUserDto);
    Task<GenericResponseModel<bool>> RegisterVisitorWithUserAsync(RegisterVisitorUserDto dto);
    Task<GenericResponseModel<CreateUserDto>> CreateUserAsync(CreateUserDto model);
    Task<GenericResponseModel<List<GetUserDto>>> GetAllUsersAsync();
    Task<GenericResponseModel<bool>> DeleteUserAsync(string userIdOrName);
    Task<GenericResponseModel<bool>> UpdateUserAsync(UpdateUserDto model);
    Task<GenericResponseModel<GetUserDto>> GetUserByIdAsync(string userIdOrName);
}
