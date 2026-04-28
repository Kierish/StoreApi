using StoreApi.Common.ErrorMessages;
using StoreApi.Common.Mappers.Auth;
using StoreApi.Common.Primitives;
using StoreApi.DTOs.Auth;
using StoreApi.Models.Identity;
using StoreApi.Repositories.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace StoreApi.Services.Auth
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;
        private readonly IAuthService _authService;

        public AccountService(IAccountRepository repo,
            IAuthService authService)
        {
            _repo = repo;
            _authService = authService;
        }

        public async Task<Result<AuthResponseDto>> LoginUserAsync(LoginDataDto dto)
        {
            var appUser = await _repo.GetUserByEmailAsync(dto.Email);

            if (appUser is null)
                return Result<AuthResponseDto>.Failure(UserErrors.InvalidCredentials());

            string userPasswordHash = appUser.PasswordHash;
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, userPasswordHash);

            if (!isPasswordValid)
                return Result<AuthResponseDto>.Failure(UserErrors.InvalidCredentials());

            var expiredTokens = await _repo.GetUsersExpiredRefreshTokensAsync(appUser.Id);

            if (expiredTokens is not null)
            {
                _repo.RemoveRangeRefreshTokens(expiredTokens);
            }

            return Result<AuthResponseDto>.Success(await GenerateAndSaveTokensAsync(appUser));
        }

        public async Task<Result<AuthResponseDto>> RegisterUserAsync(RegisterDataDto dto)
        {
            bool isUserExists = await _repo.IsUserExistsAsync(dto);

            if (isUserExists)
                return Result<AuthResponseDto>.Failure(UserErrors.UserAlreadyExists(dto.Email));

            var newUser = dto.ToEntity();

            _repo.AddUser(newUser);
            
            return Result<AuthResponseDto>.Success(await GenerateAndSaveTokensAsync(newUser));
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(AuthRequestDto dto)
        {
            var validationResult = await ValidateRefreshTokenAsync(dto);

            if (!validationResult.IsSuccess)
                return Result<AuthResponseDto>.Failure(validationResult.Error);

            var existingRefreshToken = validationResult.Data!;

            _repo.RemoveRefreshToken(existingRefreshToken);
            
            return Result<AuthResponseDto>.Success(await GenerateAndSaveTokensAsync(existingRefreshToken.User));
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

        private async Task<Result<RefreshToken>> ValidateRefreshTokenAsync(AuthRequestDto dto)
        {
            // Check 1 & 2
            var principalResult = _authService.GetPrincipalFromExpiredToken(dto.JwtToken);
            if (!principalResult.IsSuccess)
            {
                return Result<RefreshToken>.Failure(principalResult.Error);
            }

            var principal = principalResult.Data!;

            // Check 3: Jwt expiry date
            var expiryDateUnix = long.Parse(principal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateUtc > DateTime.UtcNow)
                return Result<RefreshToken>.Failure(AuthErrors.TokenNotExpired());

            var jti = principal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            // Check 4: Refresh Token existence 
            var existingRefreshToken = await _repo.GetRefreshTokenAsync(dto.RefreshToken);

            if (existingRefreshToken is null)
                return Result<RefreshToken>.Failure(AuthErrors.InvalidRefreshToken());

            // Check 5: Refresh Token expiration
            if (existingRefreshToken.DateExpire < DateTime.UtcNow)
                return Result<RefreshToken>.Failure(AuthErrors.RefreshTokenExpired());

            // Check 6: Refresh Token revoked
            if (existingRefreshToken.IsRevoked)
                return Result<RefreshToken>.Failure(AuthErrors.TokenRevoked());

            // Check 7: Validate Id
            if (existingRefreshToken.JwtId != jti)
                return Result<RefreshToken>.Failure(AuthErrors.InvalidTokenLinkage());

            return Result<RefreshToken>.Success(existingRefreshToken);
        }
    }
}
