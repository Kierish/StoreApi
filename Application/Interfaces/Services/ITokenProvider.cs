using Domain.Models.Auth;
using Domain.Results;
using System.Security.Claims;

namespace Application.Interfaces.Services
{
    public interface ITokenProvider
    {
        string GenerateToken(ApplicationUser user, string jti);
        RefreshToken GenerateRefreshToken(ApplicationUser user, string jti);
        Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}
