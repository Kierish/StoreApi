using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs.Auth
{
    public record AuthResponseDto(
        string Token,
        string RefreshToken
        );

    public record AuthRequestDto(
        string RefreshToken,
        string JwtToken
        );
}
