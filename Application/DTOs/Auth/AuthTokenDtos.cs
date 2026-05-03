using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    /// <summary>
    /// Contains the authentication tokens returned upon successful login or registration.
    /// </summary>
    /// <param name="Token">The JWT access token.</param>
    /// <param name="RefreshToken">The refresh token used to obtain a new access token.</param>
    public record AuthResponseDto(
        string Token,
        string RefreshToken
        );

    /// <summary>
    /// Represents the request payload for refreshing an expired JWT access token.
    /// </summary>
    /// <param name="RefreshToken">The active refresh token.</param>
    /// <param name="JwtToken">The expired JWT access token.</param>
    public record AuthRequestDto(
        string RefreshToken,
        string JwtToken
        );
}
