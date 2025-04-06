using PrisonManagementSystem.BL.DTOs.Identiity.Token;
using PrisonManagementSystem.BL.DTOs.ResponseModel;

public interface IAuthService
{
    Task<GenericResponseModel<TokenResponseDto>> LoginAsync(string userNameOrEmail, string password);
    Task<GenericResponseModel<TokenResponseDto>> RefreshAsync(TokenApiDto tokenApiDto);
    Task<GenericResponseModel<bool>> LogOutAsync(string userNameOrEmail);
    Task<GenericResponseModel<bool>> ResetPasswordAsync(string email, string newPassword);
    Task<GenericResponseModel<bool>> ChangeUserPasswordAsync(string userId, string oldPassword, string newPassword);
    Task<GenericResponseModel<string>> RevokeAsync();
}
