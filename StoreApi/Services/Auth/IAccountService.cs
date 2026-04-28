using StoreApi.Common.Primitives;
using StoreApi.DTOs.Auth;

namespace StoreApi.Services.Auth
{
    public interface IAccountService
    {
        Task<Result<AuthResponseDto>> LoginUserAsync(LoginDataDto dto);
        Task<Result<AuthResponseDto>> RegisterUserAsync(RegisterDataDto dto);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(AuthRequestDto dto);
    }
}
