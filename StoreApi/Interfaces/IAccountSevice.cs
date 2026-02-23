using StoreApi.DTOs;

namespace StoreApi.Interfaces
{
    public interface IAccountSevice
    {
        Task<AuthResponseDto> LoginUserAsync(LoginDataDto dto);
        Task<AuthResponseDto> RegisterUserAsync(RegisterDataDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(AuthRequestDto dto);
    }
}
