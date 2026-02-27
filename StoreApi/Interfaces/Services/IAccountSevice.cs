using StoreApi.DTOs;

namespace StoreApi.Interfaces.Services
{
    public interface IAccountSevice
    {
        Task<AuthResponseDto> LoginUserAsync(LoginDataDto dto);
        Task<AuthResponseDto> RegisterUserAsync(RegisterDataDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(AuthRequestDto dto);
    }
}
