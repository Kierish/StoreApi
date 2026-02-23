using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Interfaces;
using StoreApi.Mappers;
using StoreApi.Models.Identity;

namespace StoreApi.Services
{
    public class AccountService : IAccountSevice
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;

        public AccountService(AppDbContext context,
            IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<AuthResponseDto> LoginUserAsync(LoginDataDto dto)
        {
            var appUser = await _context.Users.FirstOrDefaultAsync(us => us.Email == dto.Email);

            if (appUser is null)
                throw new UnauthorizedException("Invalid email or password.");

            string userPasswordHash = appUser.PasswordHash;
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, userPasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid email or password.");

            var expiredTokens = await _context.RefreshTokens
                .Where(t => t.UserId == appUser.Id && t.DateExpire < DateTime.UtcNow)
                .ToListAsync();

            if (expiredTokens is not null)
            {
                _context.RefreshTokens.RemoveRange(expiredTokens);
            }

            return await GenerateAndSaveTokensAsync(appUser);
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterDataDto dto)
        {
            bool isUserExists = await _context.Users
                .AnyAsync(us => us.Email == dto.Email ||
                us.UserName == dto.UserName ||
                us.PhoneNumber == dto.PhoneNumber);

            if (isUserExists)
                throw new BadRequestException($"User already exists.");

            var newUser = UserApplicationMappers.ToEntity(dto);

            _context.Users.Add(newUser);
            
            return await GenerateAndSaveTokensAsync(newUser);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(AuthRequestDto dto)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(us => us.User)
                .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);

            if (refreshToken is null)
                throw new UnauthorizedException("Invalid refresh token.");

            if (refreshToken.DateExpire < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token expired. Please log in again.");

            _context.Remove(refreshToken);
            
            return await GenerateAndSaveTokensAsync(refreshToken.User);
        }

        private async Task<AuthResponseDto> GenerateAndSaveTokensAsync(ApplicationUser user)
        {
            string jti = Guid.NewGuid().ToString();

            var jwtToken = _authService.GenerateToken(user, jti);
            var refreshToken = _authService.GenerateRefreshToken(user, jti);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto(jwtToken, refreshToken.Token);
        }
    }
}
