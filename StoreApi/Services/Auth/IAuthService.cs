using Microsoft.Identity.Client;
using StoreApi.Common.Primitives;
using StoreApi.Models.Identity;
using System.Security.Claims;

namespace StoreApi.Services.Auth
{
    public interface IAuthService
    {
        string GenerateToken(ApplicationUser user, string jti);
        RefreshToken GenerateRefreshToken(ApplicationUser user, string jti);
        Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}
