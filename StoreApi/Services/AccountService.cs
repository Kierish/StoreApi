using Microsoft.EntityFrameworkCore;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Interfaces;
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

            string token = _authService.GenerateToken(appUser);
            var refreshToken = _authService.GenerateRefreshToken(appUser);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto(token, refreshToken.Token);
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterDataDto dto)
        {
            bool isUserExists = await _context.Users
                .AnyAsync(us => us.Email == dto.Email ||
                us.UserName == dto.UserName ||
                us.PhoneNumber == dto.PhoneNumber);

            if (isUserExists)
                throw new BadRequestException($"User already exists.");

            var newUser = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            string token = _authService.GenerateToken(newUser);
            var refreshToken = _authService.GenerateRefreshToken(newUser);

            _context.Users.Add(newUser);
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto(token, refreshToken.Token);
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

            string token = _authService.GenerateToken(refreshToken.User);
            var newRefreshToken = _authService.GenerateRefreshToken(refreshToken.User);

            _context.Remove(refreshToken);
            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto(token, newRefreshToken.Token);
        }
    }
}
