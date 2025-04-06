using PrisonManagementSystem.DAL.Entities.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PrisonManagementSystem.BL.Services.Abstractions.Identity;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using PrisonManagementSystem.BL.DTOs.Identiity.Token;

namespace PrisonManagementSystem.BL.Services.Implementations.Identity
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public TokenService(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Generate a new refresh token using random number generator
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        // Create access token and refresh token for the user
        public async Task<TokenResponseDto> CreateAccessToken(User user)
        {
            var tokenDTO = new TokenResponseDto();

            // Create a symmetric key using the secret key from the configuration
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            // Set up signing credentials with the security key and algorithm
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Prepare claims based on user data
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add user roles as claims
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Set token expiration date
            tokenDTO.TokenExpirationDate = DateTime.UtcNow.AddMinutes(15); // Access token expiration time (e.g., 15 minutes)

            // Create JWT security token
            var securityToken = new JwtSecurityToken(
                audience: _configuration["Jwt:Audience"],
                issuer: _configuration["Jwt:Issuer"],
                expires: tokenDTO.TokenExpirationDate, // Set expiration
                notBefore: DateTime.UtcNow, // Start date of the token's validity
                signingCredentials: signingCredentials,
                claims: claims
            );

            // Create the token handler to generate the JWT token string
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenDTO.Token = tokenHandler.WriteToken(securityToken);

            // Generate and include refresh token
            tokenDTO.RefreshToken = GenerateRefreshToken();

            return tokenDTO;
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

    


    }
}
