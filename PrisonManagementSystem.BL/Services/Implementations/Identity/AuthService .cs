
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using PrisonManagementSystem.BL.Services.Abstractions.Identity;
using Microsoft.AspNetCore.Identity;
using PrisonManagementSystem.DAL.Entities.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PrisonManagementSystem.DAL.Repositories.Abstractions.IRefreshTokenRepository;
using System.Net;
using Microsoft.Extensions.Configuration;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.BL.DTOs.Identiity.Token;

namespace PrisonManagementSystem.BL.Services.Implementations.Identity
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenReadRepository _refreshTokenReadRepository;
        private readonly IRefreshTokenWriteRepository _refreshTokenWriteRepository;

        public AuthService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _refreshTokenReadRepository = _unitOfWork.GetRepository<IRefreshTokenReadRepository>();
            _refreshTokenWriteRepository = _unitOfWork.GetRepository<IRefreshTokenWriteRepository>();
        }

        public async Task<GenericResponseModel<TokenResponseDto>> LoginAsync(string userNameOrEmail, string password)
        {

            if (string.IsNullOrWhiteSpace(userNameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                return GenericResponseModel<TokenResponseDto>.FailureResponse(
                    "Username or email and password are required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var user = await _userManager.FindByNameAsync(userNameOrEmail) ??
                      await _userManager.FindByEmailAsync(userNameOrEmail);

            if (user == null)
            {
                return GenericResponseModel<TokenResponseDto>.FailureResponse(
                    "Invalid username/email or password.",
                    (int)HttpStatusCode.Unauthorized);
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!signInResult.Succeeded)
            {
                return GenericResponseModel<TokenResponseDto>.FailureResponse(
                    "Invalid username/email or password.",
                    (int)HttpStatusCode.Unauthorized);
            }

            var tokenResponse = await _tokenService.CreateAccessToken(user);
            int refreshTokenExpirationDays = _configuration.GetValue<int>("JWT:RefreshTokenExpirationDays");

            RefreshToken? existingRefreshToken = await _refreshTokenReadRepository.GetSingleAsync(
            rt => rt.UserId == user.Id);

            RefreshToken refreshToken;

            if (existingRefreshToken != null)
            {
                existingRefreshToken.Token = _tokenService.GenerateRefreshToken();
                existingRefreshToken.ExpirationDate = DateTime.Now.AddDays(refreshTokenExpirationDays);
                await _refreshTokenWriteRepository.UpdateAsync(existingRefreshToken);
                refreshToken = existingRefreshToken;
            }
            else
            {
                refreshToken = new RefreshToken
                {
                    Token = _tokenService.GenerateRefreshToken(),
                    ExpirationDate = DateTime.Now.AddDays(refreshTokenExpirationDays),
                    UserId = user.Id
                };
                await _refreshTokenWriteRepository.AddAsync(refreshToken);
            }

            await _unitOfWork.CommitAsync();

            var tokenResponseDto = new TokenResponseDto
            {
                Token = tokenResponse.Token,
                RefreshToken = refreshToken.Token,
                TokenExpirationDate = tokenResponse.TokenExpirationDate,
                RefreshTokenExpirationDate = refreshToken.ExpirationDate
            };

            return GenericResponseModel<TokenResponseDto>.SuccessResponse(
                tokenResponseDto,
                (int)HttpStatusCode.OK,
                "Login successful.");

        }

        public async Task<GenericResponseModel<string>> RevokeAsync()
        {
            string userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return GenericResponseModel<string>.FailureResponse(
                    "User is not authenticated.",
                    (int)HttpStatusCode.BadRequest);
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return GenericResponseModel<string>.FailureResponse(
                    "User not found.",
                    (int)HttpStatusCode.NotFound);
            }

            var refreshToken = await _refreshTokenReadRepository.GetSingleAsync(
                x => x.User.UserName == userName && x.ExpirationDate > DateTime.Now);

            if (refreshToken == null)
            {
                return GenericResponseModel<string>.FailureResponse(
                    "No valid refresh token found.",
                    (int)HttpStatusCode.BadRequest);
            }

            await _refreshTokenWriteRepository.SoftDeleteAsync(refreshToken);
            await _unitOfWork.CommitAsync();

            return GenericResponseModel<string>.SuccessResponse(
                null,
                (int)HttpStatusCode.NoContent,
                "Refresh token revoked successfully.");

        }

        public async Task<GenericResponseModel<TokenResponseDto>> RefreshAsync(TokenApiDto tokenApiDto)
        {

            if (tokenApiDto == null || string.IsNullOrEmpty(tokenApiDto.AccessToken) || string.IsNullOrEmpty(tokenApiDto.RefreshToken))
            {
                return GenericResponseModel<TokenResponseDto>.FailureResponse(
                    "Token information is invalid.",
                    (int)HttpStatusCode.BadRequest);
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenApiDto.AccessToken);
            if (principal == null || principal.Identity?.Name == null)
            {
                return GenericResponseModel<TokenResponseDto>.FailureResponse(
                    "Access token is invalid.",
                    (int)HttpStatusCode.Unauthorized);
            }

            string userName = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return GenericResponseModel<TokenResponseDto>.FailureResponse(
                    "User not found.",
                    (int)HttpStatusCode.NotFound);
            }

            RefreshToken? refreshTokenObj = await _refreshTokenReadRepository.GetSingleAsync(
            rt => rt.UserId == user.Id);

            if (refreshTokenObj == null || refreshTokenObj.ExpirationDate <= DateTime.UtcNow)
            {
                return GenericResponseModel<TokenResponseDto>.FailureResponse(
                    "Refresh token is invalid or expired.",
                    (int)HttpStatusCode.Unauthorized);
            }

            if (refreshTokenObj.Token != tokenApiDto.RefreshToken || refreshTokenObj.UserId != user.Id)
            {
                return GenericResponseModel<TokenResponseDto>.FailureResponse(
                    "Refresh token is incorrect.",
                    (int)HttpStatusCode.BadRequest);
            }

            var tokenResponseDto = await _tokenService.CreateAccessToken(user);
            string newRefreshToken = _tokenService.GenerateRefreshToken();

            refreshTokenObj.Token = newRefreshToken;
            refreshTokenObj.ExpirationDate = DateTime.UtcNow.AddDays(7);

            await _refreshTokenWriteRepository.UpdateAsync(refreshTokenObj);
            await _unitOfWork.CommitAsync();

            tokenResponseDto.RefreshToken = refreshTokenObj.Token;
            tokenResponseDto.RefreshTokenExpirationDate = refreshTokenObj.ExpirationDate;

            return GenericResponseModel<TokenResponseDto>.SuccessResponse(
                tokenResponseDto,
                (int)HttpStatusCode.OK,
                "Tokens refreshed successfully.");

        }

        public async Task<GenericResponseModel<bool>> LogOutAsync(string userNameOrEmail)
        {

            if (string.IsNullOrWhiteSpace(userNameOrEmail))
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Username or email is required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var user = await _userManager.FindByNameAsync(userNameOrEmail) ??
                      await _userManager.FindByEmailAsync(userNameOrEmail);

            if (user == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "User not found.",
                    (int)HttpStatusCode.NotFound);
            }

            await _signInManager.SignOutAsync();

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                (int)HttpStatusCode.OK,
                "Logged out successfully.");

        }

        public async Task<GenericResponseModel<bool>> ChangeUserPasswordAsync(string userId, string oldPassword, string newPassword)
        {

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "User ID, old password, and new password are required.",
                    (int)HttpStatusCode.BadRequest);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "User not found.",
                    (int)HttpStatusCode.NotFound);
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (!changePasswordResult.Succeeded)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Password change failed.",
                    (int)HttpStatusCode.BadRequest);
            }

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                (int)HttpStatusCode.OK,
                "Password changed successfully.");

        }

        public async Task<GenericResponseModel<bool>> ResetPasswordAsync(string email, string newPassword)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "User not found.",
                    (int)HttpStatusCode.NotFound);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(resetToken))
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Password reset token not found.",
                    (int)HttpStatusCode.InternalServerError);
            }

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
            {
                return GenericResponseModel<bool>.FailureResponse(
                    "Password reset failed.",
                    (int)HttpStatusCode.BadRequest);
            }

            return GenericResponseModel<bool>.SuccessResponse(
                true,
                (int)HttpStatusCode.OK,
                "Password reset successfully.");


        }
    }
}