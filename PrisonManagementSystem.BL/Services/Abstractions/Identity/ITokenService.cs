using PrisonManagementSystem.BL.DTOs.Identiity.Token;
using PrisonManagementSystem.DAL.Entities.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Services.Abstractions.Identity
{
    public interface ITokenService
    {
   
        Task<TokenResponseDto> CreateAccessToken(User user);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    
    }
}
