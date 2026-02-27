using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Interfaces.Repositories;
using StoreApi.Models.Identity;

namespace StoreApi.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(us => us.Email == email);

        public async Task<List<RefreshToken>?> GetUsersExpiredRefreshTokensAsync(int userId)
            => await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.DateExpire < DateTime.UtcNow)
            .ToListAsync();

        public void RemoveRangeRefreshTokens(List<RefreshToken> refreshTokens)
            => _context.RefreshTokens.RemoveRange(refreshTokens);

        public async Task<bool> IsUserExistsAsync(RegisterDataDto dto)
            => await _context.Users
            .AnyAsync(us => us.Email == dto.Email ||
            us.UserName == dto.UserName ||
            us.PhoneNumber == dto.PhoneNumber);

        public void AddUser(ApplicationUser user)
            => _context.Users.Add(user);

        public void RemoveRefreshToken(RefreshToken refreshToken)
            => _context.RefreshTokens.Remove(refreshToken);

        public void AddRefreshToken(RefreshToken refreshToken)
            => _context.RefreshTokens.Add(refreshToken);

        public async Task<RefreshToken?> GetRefreshTokenAsync(string refreshToken)
            => await _context.RefreshTokens
            .Include(us => us.User)
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
