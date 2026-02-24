using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public record AuthResponseDto(
        string Token,
        string RefreshToken
        );

    public record AuthRequestDto(
        [Required] string RefreshToken,
        [Required] string JwtToken
        );
}
