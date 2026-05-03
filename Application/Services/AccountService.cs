using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Application.Mappers.Auth;
using Application.Repositories;
using Domain.ErrorMessages;
using Domain.Models.Auth;
using Domain.Results;

namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPasswordHasher _passwordHasher;

        public AccountService(IAccountRepository repo,
            ITokenProvider tokenProvider,
            IPasswordHasher passwordHasher)
        {
            _repo = repo;
            _tokenProvider = tokenProvider;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<AuthResponseDto>> LoginUserAsync(LoginDataDto dto)
        {
            var appUser = await _repo.GetUserByEmailAsync(dto.Email);

            if (appUser is null)
                return Result<AuthResponseDto>.Failure(UserErrors.InvalidCredentials());

            string userPasswordHash = appUser.PasswordHash;
            bool isPasswordValid = _passwordHasher.Verify(dto.Password, userPasswordHash);

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

            var hashedPassword = _passwordHasher.Hash(dto.Password);
            var newUser = dto.ToEntity(hashedPassword);

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

            var jwtToken = _tokenProvider.GenerateToken(user, jti);
            var refreshToken = _tokenProvider.GenerateRefreshToken(user, jti);

            _repo.AddRefreshToken(refreshToken);
            await _repo.SaveChangesAsync();

            return new AuthResponseDto(jwtToken, refreshToken.Token);
        }

        private async Task<Result<RefreshToken>> ValidateRefreshTokenAsync(AuthRequestDto dto)
        {
            // Check 1 & 2
            var principalResult = _tokenProvider.GetPrincipalFromExpiredToken(dto.JwtToken);
            if (!principalResult.IsSuccess)
            {
                return Result<RefreshToken>.Failure(principalResult.Error);
            }

            var principal = principalResult.Data!;

            // Check 3: Jwt expiry date
            var expiryDateUnix = long.Parse(principal.Claims.Single(c => c.Type == "exp").Value);
            var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateUtc > DateTime.UtcNow)
                return Result<RefreshToken>.Failure(AuthErrors.TokenNotExpired());

            var jti = principal.Claims.Single(c => c.Type == "jti").Value;

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
