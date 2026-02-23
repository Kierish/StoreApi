using Microsoft.Identity.Client;
using StoreApi.Models.Identity;

namespace StoreApi.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(ApplicationUser user);
        RefreshToken GenerateRefreshToken(ApplicationUser user);
    }
}
