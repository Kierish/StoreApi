using System.ComponentModel.DataAnnotations;

namespace StoreApi.DTOs
{
    public record AuthResponseDto(
        [Required] string Token,
        [Required] string RefreshToken
        );

    public record AuthRequestDto(
        string RefreshToken
        );
}
