using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Interfaces;
using StoreApi.Mappers;
using StoreApi.Models.Identity;
using System.IdentityModel.Tokens.Jwt;

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
            var existingRefreshToken = await ValidateRefreshTokenAsync(dto);

            _context.Remove(existingRefreshToken);
            
            return await GenerateAndSaveTokensAsync(existingRefreshToken.User);
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

        private async Task<RefreshToken> ValidateRefreshTokenAsync(AuthRequestDto dto)
        {
            // Check 1 & 2
            var principal = _authService.GetPrincipalFromExpiredToken(dto.JwtToken);

            // Check 3: Jwt expiry date
            var expiryDateUnix = long.Parse(principal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateUtc > DateTime.UtcNow)
                throw new BadRequestException("This token hasn't expired yet.");

            var jti = principal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            // Check 4: Refresh Token existence 
            var existingRefreshToken = await _context.RefreshTokens
                .Include(us => us.User)
                .FirstOrDefaultAsync(t => t.Token == dto.RefreshToken);

            if (existingRefreshToken is null)
                throw new UnauthorizedException("Invalid refresh token.");

            // Check 5: Refresh Token expiration
            if (existingRefreshToken.DateExpire < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh token expired.");

            // Check 6: Refresh Token revoked
            if (existingRefreshToken.IsRevoked)
                throw new UnauthorizedException("Token revoked.");

            // Check 7: Validate Id
            if (existingRefreshToken.JwtId != jti)
                throw new UnauthorizedException("Invalid token linkage.");

            return existingRefreshToken;
        }
    }
}
