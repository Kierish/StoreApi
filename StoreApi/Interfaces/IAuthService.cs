using Microsoft.Identity.Client;
using StoreApi.Models.Identity;

namespace StoreApi.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(ApplicationUser user, string jti);
        RefreshToken GenerateRefreshToken(ApplicationUser user, string jti);
    }
}
