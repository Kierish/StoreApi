using Application.DTOs.Auth;
using Domain.Results;

namespace Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<Result<AuthResponseDto>> LoginUserAsync(LoginDataDto dto);
        Task<Result<AuthResponseDto>> RegisterUserAsync(RegisterDataDto dto);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(AuthRequestDto dto);
    }
}
