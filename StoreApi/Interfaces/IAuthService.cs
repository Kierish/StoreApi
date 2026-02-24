using Microsoft.Identity.Client;
using StoreApi.Models.Identity;
using System.Security.Claims;

namespace StoreApi.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(ApplicationUser user, string jti);
        RefreshToken GenerateRefreshToken(ApplicationUser user, string jti);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
