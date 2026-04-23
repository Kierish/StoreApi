using System.Security.Claims;
using Domain.Entities.Identity;

namespace Application.Interfaces.Services
{
    public interface IAuthService
    {
        string GenerateToken(ApplicationUser user, string jti);
        RefreshToken GenerateRefreshToken(ApplicationUser user, string jti);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
