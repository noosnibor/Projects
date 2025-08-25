using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Web.Api.Data;
using Web.Api.Security;

namespace Web.Api.Services;

public class TokenService(JwtOption jwtOption, UserRepository userRepository, RoleRepository roleRepository, TokenRepository tokenRepository)
{
    #region Private Variables
    private readonly JwtOption jwtOption = jwtOption;
    private readonly UserRepository userRepository = userRepository;
    private readonly RoleRepository roleRepository = roleRepository;
    private readonly TokenRepository tokenRepository = tokenRepository;
    #endregion

    #region Issue Token
    public async Task<(string access, string refresh, DateTime accessExp, DateTime refreshExp)> Issue(Guid userId, string device, string ip)
    {
        // Variables
        var role = await userRepository.GetRoleName(userId);
        var now = DateTime.UtcNow;
        var accessExp = now.AddMinutes(jwtOption.AccessTokenMinutes);
        var refreshExp = now.AddDays(jwtOption.RefreshTokenDays);

        // Set Calims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        // Get the roles
        claims.AddRange(role.Select(r => new Claim(ClaimTypes.Role, r)));

        // Credentials
        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SigningKey)), SecurityAlgorithms.HmacSha512);

        // Jwt Token
        var jwt = new JwtSecurityToken(jwtOption.Issuer, jwtOption.Audience, claims, now, accessExp, creds);

        // Access token
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        // Refresh Token
        var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        // Save the refresh token
        await tokenRepository.CreateRefreshToken(userId, refresh, now, refreshExp, device, ip);

        // Return Issue Token
        return (accessToken, refresh, accessExp, refreshExp);

    }
    #endregion

    #region Refresh Token
    public async Task<(Guid userId, string access, string refresh)> Refresh(string refreshToken, string device, string ip)
    {
        // Check if the refresh token is valid
        var existing = await tokenRepository.FindActiveRefreshToken(refreshToken) ?? throw new UnauthorizedAccessException("Invalid refresh token");

        // Rotate token
        var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        // Update refresh token
        await tokenRepository.RotateRefreshToken(existing.Id, newRefreshToken);

        // Issue Token
        var(access, _, _, _) = await Issue(existing.UserId, device, ip);

        // Return refresh token
        return (existing.UserId, access, newRefreshToken);

    }
    #endregion
}
