using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreApi.Data;
using StoreApi.DTOs;
using StoreApi.Exceptions;
using StoreApi.Interfaces.Repositories;
using StoreApi.Interfaces.Services;
using StoreApi.Mappers;
using StoreApi.Models.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace StoreApi.Services
{
    public class AccountService : IAccountSevice
    {
        private readonly IAccountRepository _repo;
        private readonly IAuthService _authService;

        public AccountService(IAccountRepository repo,
            IAuthService authService)
        {
            _repo = repo;
            _authService = authService;
        }

        public async Task<AuthResponseDto> LoginUserAsync(LoginDataDto dto)
        {
            var appUser = await _repo.GetUserByEmailAsync(dto.Email);

            if (appUser is null)
                throw new UnauthorizedException("Invalid email or password.");

            string userPasswordHash = appUser.PasswordHash;
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, userPasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid email or password.");

            var expiredTokens = await _repo.GetUsersExpiredRefreshTokensAsync(appUser.Id);

            if (expiredTokens is not null)
            {
                _repo.RemoveRangeRefreshTokens(expiredTokens);
            }

            return await GenerateAndSaveTokensAsync(appUser);
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterDataDto dto)
        {
            bool isUserExists = await _repo.IsUserExistsAsync(dto);

            if (isUserExists)
                throw new BadRequestException($"User already exists.");

            var newUser = dto.ToEntity();

            _repo.AddUser(newUser);
            
            return await GenerateAndSaveTokensAsync(newUser);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(AuthRequestDto dto)
        {
            var existingRefreshToken = await ValidateRefreshTokenAsync(dto);

            _repo.RemoveRefreshToken(existingRefreshToken);
            
            return await GenerateAndSaveTokensAsync(existingRefreshToken.User);
        }

        private async Task<AuthResponseDto> GenerateAndSaveTokensAsync(ApplicationUser user)
        {
            string jti = Guid.NewGuid().ToString();

            var jwtToken = _authService.GenerateToken(user, jti);
            var refreshToken = _authService.GenerateRefreshToken(user, jti);

            _repo.AddRefreshToken(refreshToken);
            await _repo.SaveChangesAsync();

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
            var existingRefreshToken = await _repo.GetRefreshTokenAsync(dto.RefreshToken);

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
