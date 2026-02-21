using Microsoft.Identity.Client;
using StoreApi.Models;

namespace StoreApi.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(ApplicationUser user);
    }
}
