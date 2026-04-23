using Domain.Entities.Identity;
using StoreApi.DTOs;

namespace StoreApi.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        void RemoveRangeRefreshTokens(List<RefreshToken> refreshTokens);
        Task<bool> IsUserExistsAsync(RegisterDataDto dto);
        void AddUser(ApplicationUser user);
        void RemoveRefreshToken(RefreshToken refreshToken);
        void AddRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken);
        Task SaveChangesAsync();
    }
}
